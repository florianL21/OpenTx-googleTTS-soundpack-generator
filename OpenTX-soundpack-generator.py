import argparse
import os
import shutil
import progressbar

from google.cloud import texttospeech
from pydub import AudioSegment
from openpyxl import load_workbook

client = texttospeech.TextToSpeechClient()

filepath = "";

class pcolors:
	HEADER = '\033[95m'
	OKBLUE = '\033[94m'
	OKGREEN = '\033[92m'
	WARNING = '\033[93m'
	FAIL = '\033[91m'
	ENDC = '\033[0m'
	BOLD = '\033[1m'
	UNDERLINE = '\033[4m'
	BGGREEN = '\x1b[6;30;42m'

def synthesize_ssml(ssml_text, output_file_name):
	"""Synthesizes speech from the input text of ssml.
	Note: documentation from google for ssml
		https://cloud.google.com/text-to-speech/docs/ssml
	"""

	input_text = texttospeech.types.SynthesisInput(ssml=ssml_text)

	audio_config = texttospeech.types.AudioConfig(
        audio_encoding=texttospeech.enums.AudioEncoding.MP3)

	if os.path.isfile(output_file_name + ".wav") and (not args.override):
		print(pcolors.OKBLUE + "<-- Skipped" + pcolors.ENDC)
	else:
		response = client.synthesize_speech(input_text, voice, audio_config)

		# The response's audio_content is binary.
		with open('output.mp3', 'wb') as out:
			out.write(response.audio_content)
		sound = AudioSegment.from_mp3("output.mp3")
		sound = sound.set_channels(1)
		sound = sound.set_frame_rate(32000)
		sound.export(output_file_name + ".wav", format="wav")
		print("")


def create_filesystem():
	if args.remove and os.path.exists(filepath):
		print(pcolors.WARNING + "Removing old files and folders..." + pcolors.ENDC)
		shutil.rmtree("SOUNDS/" + str(args.language))
		os.makedirs(filepath)
	elif not os.path.exists(filepath):
		os.makedirs(filepath)
		print(pcolors.WARNING + "Directories do not exist, creating filepath" + pcolors.ENDC)
	else:
		print(pcolors.WARNING + "Filepath already exists" + pcolors.ENDC)

def shorten_text(long_text, max_length):
	if(len(str(long_text)) >= max_length):
		return str(long_text)[:max_length-4] + "..."
	else:
		return str(long_text)
		
def create_voice_from_list():
	#find the last row:
	lastRow = ws.max_row
	for i in range(2, ws.max_row):
		if (ws.cell(row=i, column=1).value == None) and (ws.cell(row=i, column=2).value == None) and (ws.cell(row=i, column=3).value == None):
			lastRow = i
			break
	ignoredRows = ws.max_row - lastRow
	if ignoredRows <> 0:
		print(pcolors.UNDERLINE + "Ignored " + str(ignoredRows) + " Empty rows from xlsx file" + pcolors.ENDC)
	else:
		lastRow += 1
	print("Generating " + str(lastRow-2) + " Files")
	print(pcolors.BGGREEN + "\tFilename:\tDescription:\t\t\tText:" + pcolors.ENDC)
	for r in progressbar.progressbar(range(2, lastRow), redirect_stdout=True):
		print "\t{0:<10s}\t{1:<30s}\t{2:<40s}".format(shorten_text(ws.cell(row=r, column=1).value, 10),
			shorten_text(ws.cell(row=r, column=2).value, 30),shorten_text(ws.cell(row=r, column=3).value, 40)),
		synthesize_ssml('<speak>' + str(ws.cell(row=r, column=3).value) + '</speak>', filepath + str(ws.cell(row=r, column=1).value))



if __name__ == '__main__':
	parser = argparse.ArgumentParser(
        description=__doc__,
        formatter_class=argparse.RawDescriptionHelpFormatter)
	parser.add_argument('-f', '--file',
                       help='The name of the input .xlsx file')
	parser.add_argument('-l', '--language',
                       help='If formated correctly this should be the language code. For example en for english')
	parser.add_argument('-v', '--voice', default='en-US-Wavenet-D',
                       help='Define the name of the TTS voice')
	parser.add_argument('-r', '--remove', action='store_true',
                       help='Use to clear all directories and start over fresh.')
	parser.add_argument('-o', '--override', action='store_true',
                       help='Use to enable overide if a file already exists. Use if you want to regenerate all voices from the excel file without affecting the folder structure. This will NOT delete any files!')
	parser.add_argument('-s', '--singleLine', type=int, default=0,
                       help='Use this option to regenerate one single line of the excel file. Add a minus to the line number to regenerate system voices, a positive value will regenerate user voices')
	args = parser.parse_args()
	
	# Note: the voice can also be specified by name.
    # Names of voices can be retrieved with client.list_voices().
	voice = texttospeech.types.VoiceSelectionParams(
        language_code=args.voice[:5],
        name=args.voice)
	wb = load_workbook(str(args.file), data_only=True)
	
	filepath = "SOUNDS/" + str(args.language) + "/SYSTEM/"
	create_filesystem()
	if args.singleLine == 0:
		#generate system sounds
		print(pcolors.BOLD + "Generating system voices" + pcolors.ENDC)
		ws = wb[str(args.language) + '-system']
		create_voice_from_list()

		#generate user sounds
		filepath = "SOUNDS/" + str(args.language) + "/"
		print(pcolors.BOLD + "Generating user defined voices" + pcolors.ENDC)
		ws = wb[str(args.language) + '-user']
		create_voice_from_list()
	else:
		if args.singleLine > 0:
			filepath = "SOUNDS/" + str(args.language) + "/"
			print(pcolors.BOLD + "Generating user defined voices" + pcolors.ENDC)
			ws = wb[str(args.language) + '-user']
			print(pcolors.BGGREEN + "\tFilename:\tDescription:\t\t\tText:" + pcolors.ENDC)
			print "\t{0:<10s}\t{1:<30s}\t{2:<40s}".format(shorten_text(ws.cell(row=args.singleLine, column=1).value, 10),
				shorten_text(ws.cell(row=args.singleLine, column=2).value, 30),shorten_text(ws.cell(row=args.singleLine, column=3).value, 40)),
			synthesize_ssml('<speak>' + str(ws.cell(row=args.singleLine, column=3).value) + '</speak>', filepath + str(ws.cell(row=args.singleLine, column=1).value))
		else:
			filepath = "SOUNDS/" + str(args.language) + "/SYSTEM/"
			print(pcolors.BOLD + "Generating system voices" + pcolors.ENDC)
			ws = wb[str(args.language) + '-system']
			print(pcolors.BGGREEN + "\tFilename:\tDescription:\t\t\tText:" + pcolors.ENDC)
			print "\t{0:<10s}\t{1:<30s}\t{2:<40s}".format(shorten_text(ws.cell(row=-args.singleLine, column=1).value, 10),
				shorten_text(ws.cell(row=-args.singleLine, column=2).value, 30), shorten_text(ws.cell(row=-args.singleLine, column=3).value, 40)),
			synthesize_ssml('<speak>' + str(ws.cell(row=-args.singleLine, column=3).value) + '</speak>', filepath + str(ws.cell(row=-args.singleLine, column=1).value))
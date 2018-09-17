#You can change the default voice by editing the line below:
standardVoice = "Wavenet-A"


import argparse
import os
import shutil
import progressbar

from google.cloud import texttospeech
from pydub import AudioSegment
from openpyxl import load_workbook


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
		if os.path.isfile(output_file_name + ".wav"):
			print(pcolors.OKGREEN + "<-- Overwritten" + pcolors.ENDC)
		else:
			print("")
		
		response = client.synthesize_speech(input_text, voice, audio_config)
		# The response's audio_content is binary.
		with open('output.mp3', 'wb') as out:
			out.write(response.audio_content)
		sound = AudioSegment.from_mp3("output.mp3")
		sound = sound.set_channels(1)
		sound = sound.set_frame_rate(32000)
		sound.export(output_file_name + ".wav", format="wav")


def create_filesystem(filepath):
	if args.remove and os.path.exists(filepath):
		print(pcolors.WARNING + "Removing old files and folders..." + pcolors.ENDC)
		shutil.rmtree("SOUNDS/" + str(args.language).split('-')[0])
		os.makedirs(filepath)
	elif not os.path.exists(filepath):
		os.makedirs(filepath)
		print(pcolors.WARNING + "Directories do not exist, creating filepath" + pcolors.ENDC)
	else:
		print(pcolors.WARNING + "Filepath already exists" + pcolors.ENDC)

def shorten_text(long_text, max_length):
	long_text = unicode(long_text)
	if(len(long_text) >= max_length):
		return long_text[:max_length-4] + "..."
	else:
		return long_text
	
	
def find_last_row(worksheet):
	lastRow = worksheet.max_row
	for i in range(2, worksheet.max_row):
		if (worksheet.cell(row=i, column=1).value == None) and (worksheet.cell(row=i, column=2).value == None) and (worksheet.cell(row=i, column=3).value == None):
			lastRow = i
			break
	return lastRow
		
def create_voice_from_list(filepath, worksheet):
	lastRow = find_last_row(worksheet)
	ignoredRows = worksheet.max_row - lastRow
	if ignoredRows <> 0:
		print(pcolors.UNDERLINE + "Ignored " + str(ignoredRows) + " Empty rows from file" + pcolors.ENDC)
	else:
		lastRow += 1
	print("Generating " + str(lastRow - 2) + " Files")
	print(pcolors.BGGREEN + "\tFilename:\tDescription:\t\t\tText:" + pcolors.ENDC)
	for r in progressbar.progressbar(range(2, lastRow), redirect_stdout=True):
		print u"\t{0:<10s}\t{1:<30s}\t{2:<40s}".format(shorten_text(worksheet.cell(row=r, column=1).value, 10),
			shorten_text(worksheet.cell(row=r, column=2).value, 30),shorten_text(worksheet.cell(row=r, column=3).value, 40)),
		synthesize_ssml('<speak>' + unicode(worksheet.cell(row=r, column=3).value) + '</speak>', filepath + unicode(worksheet.cell(row=r, column=1).value))

def create_voice_from_lines(start_index, end_index, filepath, worksheet):
	print(pcolors.BGGREEN + "\tFilename:\tDescription:\t\t\tText:" + pcolors.ENDC)
	for r in progressbar.progressbar(range(start_index, end_index), redirect_stdout=True):
		print u"\t{0:<10s}\t{1:<30s}\t{2:<40s}".format(shorten_text(worksheet.cell(row=r, column=1).value, 10),
				shorten_text(worksheet.cell(row=r, column=2).value, 30),shorten_text(worksheet.cell(row=r, column=3).value, 40)),
		if not ((worksheet.cell(row=r, column=1).value == None) and (worksheet.cell(row=r, column=2).value == None) and (worksheet.cell(row=r, column=3).value == None)) and r >= 2:
			synthesize_ssml('<speak>' + unicode(worksheet.cell(row=r, column=3).value) + '</speak>', filepath + unicode(worksheet.cell(row=r, column=1).value))
		else:
			print(pcolors.WARNING + "<-- Ignored" + pcolors.ENDC)
		


if __name__ == '__main__':

	parser = argparse.ArgumentParser(
        description=__doc__,
        formatter_class=argparse.RawDescriptionHelpFormatter)
	parser.add_argument('-f', '--file', required=True,
                       help='The name of the input file')
	parser.add_argument('-l', '--language', required=True,
                       help='If formated correctly this should be the language code. For example en-US for english')
	parser.add_argument('-v', '--voice', nargs='?',
                       help='Define the name of the TTS voice')
	parser.add_argument('-r', '--remove', action='store_true',
                       help='Use to clear all directories and start over fresh.')
	parser.add_argument('-o', '--override', action='store_true',
                       help='Use to enable overide if a file already exists. Use if you want to regenerate all voices from the excel file without affecting the folder structure. This will NOT delete any files!')
	parser.add_argument('-s', '--singleLine', type=int, nargs='?',
                       help='Use this option to regenerate one single line of the excel file. Add a minus to the line number to regenerate system voices, a positive value will regenerate user voices')
	parser.add_argument('-b', '--beginAtLine', type=int, nargs='?',
                       help='Use this to regenerate voices for multiple lines that you define. If you specify --beginAtLine you also have to specify --endAtLine. As with singleLine a negative number defines a line from system voices.')
	parser.add_argument('-e', '--endAtLine', type=int, nargs='?',
                       help='Use this to regenerate voices for multiple lines that you define. If you specify --endAtLine you also have to specify --beginAtLine. As with singleLine a negative number defines a line from system voices.')
	args = parser.parse_args()
	
	# Pre check arguments to make sure thet nothing is missconfigured.
	if (args.singleLine is not None and (args.beginAtLine is not None or args.endAtLine is not None)):
		print(pcolors.FAIL + "You cannot try to gereate a sinle line and multiple lines at the same time. Please remove either the -s(--singleLine) argument or both of -b(--beginAtLine) AND -e(--endAtLine)" + pcolors.ENDC)
		exit()
	if (args.beginAtLine is not None and args.endAtLine is None):
		print(pcolors.FAIL + "The argument --beginAtLine requires --endAtLine" + pcolors.ENDC)
		exit()
	elif (args.beginAtLine is None and args.endAtLine is not None):
		print(pcolors.FAIL + "The argument --endAtLine requires --beginAtLine" + pcolors.ENDC)
		exit()
	elif((args.beginAtLine < 0 and args.endAtLine < 0) and args.beginAtLine < args.endAtLine):
		print(pcolors.FAIL + "Because you are generating system sounds --beginAtLine must be greater than --endAtLine" + pcolors.ENDC)
		exit()
	elif(args.beginAtLine > args.endAtLine) and not (args.beginAtLine < 0 and args.endAtLine < 0):
		print(pcolors.FAIL + "--endAtLine must be greater than --beginAtLine" + pcolors.ENDC)
		exit()
	elif (args.beginAtLine is not None and args.endAtLine is not None):
		multiLine = 1
	else:
		multiLine = 0
		
	try:
		print("\n\n*************************************************************\n*** Starting up...\n*************************************************************\n")
		
		# Initialize the google cloud TTS API
		usingDefaultVoice = args.voice is None
		if usingDefaultVoice:
			usedVoice = str(args.language) + "-" + standardVoice
		else:
			usedVoice = str(args.language) + "-" + args.voice
		client = texttospeech.TextToSpeechClient()
		voice = texttospeech.types.VoiceSelectionParams(
			language_code = args.language,
			name = usedVoice)
		
		# Check for errors in the voice definition:
		voices = client.list_voices()
		voiceMatch = 0
		for oneVoice in voices.voices:
			#check if any voice mathes the request
			if(voice.name == oneVoice.name):
				voiceMatch = 1
				break
		
		if voiceMatch :
			print("Using '" + voice.name + "' for voice output")
		else:
			if usingDefaultVoice:
				# Try to find a voice that is supports the desired language:
				print(pcolors.WARNING + "The defined default voice does not exist. Using the first voice for your language in the list: " + pcolors.ENDC)
				selectedCountry_code = 0
				selectedName = 0
				for oneVoice in voices.voices:
					selectedCountry_code = 0
					for language_code in oneVoice.language_codes:
						if(language_code == str(args.language)):
							selectedCountry_code = language_code
							break
					if(selectedCountry_code is not 0):
						selectedName = oneVoice.name
						break
				if((selectedName is not 0) and (selectedCountry_code is not 0)):
					voice = texttospeech.types.VoiceSelectionParams(
						language_code = selectedCountry_code,
						name = selectedName)
					print(voice.name)
				else:
					print(pcolors.FAIL + "Sorry, your Language is not supported by Google cloud TTS" + pcolors.ENDC)
					exit()
			else:
				print(pcolors.FAIL + "No voice found matching your request of: " + voice.name + pcolors.ENDC)
				print(pcolors.UNDERLINE + "Here is a list of all supported voices:"  + pcolors.ENDC)
				for oneVoice in voices.voices:
					# Display the voice's name.
					print('Name: {}'.format(oneVoice.name))
				exit()
		
		# Check if the input file exists:
		if not os.path.isfile(str(args.file)):
			print(pcolors.FAIL + "This file does not exist: " + str(args.file) + pcolors.ENDC)
			exit()
		
		# Try to determine which file type it is:
		try:
			inputFile = str(args.file)
			fileExention = inputFile.split('.')[len(inputFile.split('.')) - 1]
		except:
			print(pcolors.FAIL + "Something is wrong with your filename" + pcolors.ENDC)
			exit()
			
		# Check if file is a csv or a xlsx file. If you need checks for other file extentions add them here:
		if fileExention == 'csv':
			print(pcolors.FAIL + "CSV files are not yet supported..." + pcolors.ENDC)
			exit()
		elif fileExention == 'xlsx':
			wb = load_workbook(inputFile, data_only=True)
		else:
			print(pcolors.FAIL + "Unsupported file" + pcolors.ENDC)
			exit()
		
		
		# Start to process the request:
		filepathSystem = "SOUNDS/" + str(args.language).split('-')[0] + "/SYSTEM/"
		filepathUser = "SOUNDS/" + str(args.language).split('-')[0] + "/"
		create_filesystem(filepathSystem)
	
	
		if(args.singleLine is None and multiLine == 0):
			#generate system sounds
			print(pcolors.BOLD + "Generating system voices" + pcolors.ENDC)
			ws = wb[str(args.language) + '-system']
			create_voice_from_list(filepathSystem, ws)

			#generate user sounds
			print(pcolors.BOLD + "Generating user defined voices" + pcolors.ENDC)
			ws = wb[str(args.language) + '-user']
			create_voice_from_list(filepathUser, ws)
		elif(args.singleLine is not None):
			if(args.singleLine > 0):
				print(pcolors.BOLD + "Generating user defined voice from line {}".format(str(args.singleLine)) + pcolors.ENDC)
				ws = wb[str(args.language) + '-user']
				create_voice_from_lines(args.singleLine, args.singleLine + 1, filepathUser, ws)
			else:
				print(pcolors.BOLD + "Generating system voice from line {}".format(str(-args.singleLine)) + pcolors.ENDC)
				ws = wb[str(args.language) + '-system']
				create_voice_from_lines(-args.singleLine, (-args.singleLine) + 1, filepathSystem, ws)
		elif(multiLine == 1):
			if(args.beginAtLine > 0 and args.endAtLine > 0):
				print(pcolors.BOLD + "Generating user defined voices from line {} to {}".format(str(args.beginAtLine), str(args.endAtLine)) + pcolors.ENDC)
				ws = wb[str(args.language) + '-user']
				create_voice_from_lines(args.beginAtLine, args.endAtLine, filepathUser, ws)
			elif(args.beginAtLine < 0 and args.endAtLine < 0):
				print(pcolors.BOLD + "Generating system voices from line {} to {}".format(str(-args.beginAtLine), str(-args.endAtLine)) + pcolors.ENDC)
				ws = wb[str(args.language) + '-system']
				create_voice_from_lines(-args.beginAtLine, -args.endAtLine, filepathSystem, ws)
			else:
				ws = wb[str(args.language) + '-system']
				lastRow = find_last_row(ws)
				print(pcolors.BOLD + "Generating system voices from line {} to {}".format(str(-args.beginAtLine), str(lastRow)) + pcolors.ENDC)
				create_voice_from_lines(-args.beginAtLine, lastRow, filepathSystem, ws)
				
				print(pcolors.BOLD + "Generating user defined voices from line 2 to {}".format(str(args.endAtLine)) + pcolors.ENDC)
				ws = wb[str(args.language) + '-user']
				create_voice_from_lines(2, args.endAtLine + 1, filepathUser, ws)
				
				
		print("\n\n*************************************************************\n*** Done!!!\n*************************************************************\n")
	except KeyboardInterrupt:
		print(pcolors.FAIL + "Exiting script" + pcolors.ENDC)
		exit()
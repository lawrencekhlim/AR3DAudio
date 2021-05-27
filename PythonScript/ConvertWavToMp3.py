import subprocess
import os

path1 = "../Assets/Resources/allOutput/Blood, Sweat, No Tears - Jeremy Korpas/bass.wav"

folders = [
	"Blood, Sweat, No Tears - Jeremy Korpas",
	"Do or Die - Dougie Wood", 
	"Ever Felt pt.2 - Otis McDonald", 
	"Game Set Match - Riot", 
	"Gemini Robot - Bird Creek", 
	"Here, If You're Going - Otis McDonald", 
	"La, La, La - Otis McDonald", 
	"Life is Good - Magic In The Other", 
	"Mariachiando - Doug Maxwell_Jimmy Fontanez", 
	"O Christmas Tree (Vocals) - Jingle Punks", 
	"Poured - Lauren Duski", 
	"Up on the Housetop - E's Jammy Jams"
	]
instruments = ['bass', 'drums', 'other', 'piano', 'vocals']
#suffix = ['_left', '_right']
suffix = ['']

##### Encode .wav as mp3
# path = "../Assets/Resources/allOutput"
# for folder in folders:
# 	for instrument in instruments:
# 		for s in suffix:
# 			filepath = path + "/" + folder + "/" + instrument + s + ".wav"
# 			if (not os.path.isfile(filepath)):
# 				print("ERROR: Could not find " + filepath)
# 			else:
# 				cmd = ['lame', '--preset', 'insane',  filepath]
# 				subprocess.run(cmd, stdout=subprocess.PIPE, stderr=subprocess.PIPE)



###### delete .wav for _left and _right
path = "../Assets/Resources/allOutput"
for folder in folders:
	for instrument in instruments:
		for s in suffix:
			filepath = path + "/" + folder + "/" + instrument + s + ".wav"
			if (not os.path.isfile(filepath)):
				print("ERROR: Could not find " + filepath)
			else:
				try:
					os.remove(filepath)
				except Exception as e:
					print(e)

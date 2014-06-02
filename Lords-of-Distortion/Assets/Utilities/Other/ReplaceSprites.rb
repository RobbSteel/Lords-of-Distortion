require 'shoes'

def replaceSprites(animation, spriteSheet)
	if(animation == nil || spriteSheet == nil)
		return
	end

	spriteGuid = ""
	File.open(spriteSheet.to_s + ".meta") do |file|
		file.each do |line|
			 if line =~ /\Aguid/ #\A checks beginning of line 
			 	spriteGuid = line.strip #store guid line in ram
			 	break
			 end
		end
	end 

	animationFile = File.read(animation)
	puts "Replaced " + (animationFile.scan(/guid.*?,\s+/).count / 2).to_s + " frames" #match thigns between guid and a comma
	updatedAnimation = animationFile.gsub(/guid.*?,\s+/, spriteGuid + ", ")

	File.open(animation, "w") {
		|file| file.puts updatedAnimation
	}

end

Shoes.app{
	@status = title "Select files.."
	@openAnim = button "Animation"
	@openSprite = button "Replacement Sprite Sheet"
	animationFile = nil
	@openAnim.click {
		animationFile = ask_open_file
		@status.replace File.basename(animationFile)
	}
	spriteSheet = nil
	@openSprite.click {
		spriteSheet = ask_open_file
		@status.replace File.basename(spriteSheet)
	}
	@process = button "Replace"

	@process.click {
		replaceSprites(animationFile, spriteSheet)
		@status.replace "Done"
	}
}
#replaceSprites(ARGV[0], ARGV[1])
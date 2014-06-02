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


replaceSprites(ARGV[0], ARGV[1])
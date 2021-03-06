require 'fileutils'
require 'shoes'

#Usage: CopyMeta File who's meta we want, file who's meta we want to replace
#Note, both files must have a meta file.
def copyMeta(from, to)

	if from == nil || to == nil
		return nil
	end
	#open "to" file
	originalGuid = ""
	File.open(to.to_s + ".meta") do |file|
		file.each do |line|
			 if line =~ /\Aguid/ #\A checks beginning of line 
			 	puts ("original " + line)
			 	originalGuid = line #store guid line in ram
			 	break
			 end
		end
	end #close
	
	#overwrite to file with from file
	FileUtils.copy(from + ".meta", to + ".meta")

	copiedFile = File.read(to + ".meta")
	#puts "taking meta from " + copiedFile[/\Aguid*/].to_s
	restoredFile = copiedFile.sub(/\Aguid*\D/, originalGuid) #\D checks up to end of line
	#open to file again
	#replace with original guid
	File.open(to + ".meta", "w") {
		|file| file.puts restoredFile
	}
end


#copyMeta(ARGV[0], ARGV[1])

Shoes.app{
	@status = title "Select files.."
	@openAnim = button "Original"
	@openSprite = button "Recolored"
	sheet1 = nil
	@openAnim.click {
		sheet1 = ask_open_file
		@status.replace File.basename(sheet1)
	}
	sheet2 = nil
	@openSprite.click {
		sheet2 = ask_open_file
		@status.replace File.basename(sheet2)
	}

	@process = button "Copy Meta"

	@process.click {
		copyMeta(sheet1, sheet2)
		@status.replace "Done"
	}
}
require 'fileutils'

#Read all files in current directory. Files that have .anim will be copied into a
#new subdirecory and appended with the directory name.
def colorCopy(color)
	Dir.mkdir(color)

	files = Dir.entries(".")

	files.each {
		 |x|
		 extension =  File.extname(x)
		 if extension == ".anim"
		 	baseName = x.chomp(extension)
		 	FileUtils.copy(x, color + "/" + baseName + "_" + color + extension)
		 end
	}
end

ARGV.each do |a|
	colorCopy(a)
end


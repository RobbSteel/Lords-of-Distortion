require 'fileutils'

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
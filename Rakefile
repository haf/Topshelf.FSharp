require 'bundler/setup'
require 'fileutils'
require 'albacore'
require 'albacore/tasks/versionizer'
require 'albacore/tasks/release'

directory 'build'

Configuration = ENV['CONFIGURATION'] || 'Release'

build :clean_sln do |b|
  b.prop 'Configuration', Configuration
  b.sln = 'src/Topshelf.FSharp.sln'
  b.target = 'Clean'
end

desc 'clean the solution directory'
task :clean => :clean_sln do
  puts 'Removing build/'
  FileUtils.rm_rf './build/'
end

Albacore::Tasks::Versionizer.new :versioning

asmver_files :asmver => :versioning do |a|
  a.files = FileList['**/*proj']
  a.attributes assembly_description: 'Topshelf F# API',
               assembly_configuration: Configuration,
               assembly_copyright: "(c) #{Time.now.year} by Henrik Feldt",
               assembly_version: ENV['LONG_VERSION'],
               assembly_file_version: ENV['LONG_VERSION'],
               assembly_informational_version: ENV['BUILD_VERSION']
end

namespace :bin do
  nugets_restore :restore do |p|
    p.out = 'src/packages'
    p.exe = 'tools/NuGet.exe'
  end

  build :compile_quick do |b|
    b.prop 'Configuration', Configuration
    b.sln = 'src/Topshelf.FSharp.sln'
  end

  desc 'compile Topshelf.FSharp'
  task :compile => [:restore, 'build', :asmver, :compile_quick]

  directory 'build/pkg'

  nugets_pack :nugets_quick => 'build/pkg' do |p|
    p.configuration = Configuration
    p.files   = FileList['src/**/*.{csproj,fsproj,nuspec}'].
      exclude(/Tests|Specs|Example/)
    p.out     = 'build/pkg'
    p.exe     = 'tools/NuGet.exe'
    p.with_metadata do |m|
      m.description = 'Topshelf F# API'
      m.authors = 'Henrik Feldt'
      m.version = ENV['NUGET_VERSION']
      m.project_url = 'https://github.com/haf/Topshelf.FSharp'
    end
  end

  desc 'package nugets - finds all projects and package them'
  task :nugets => [:compile, :nugets_quick]
end

task :ensure_key do
  raise "missing NUGET_KEY" unless ENV['NUGET_KEY']
end

Albacore::Tasks::Release.new :release,
                             pkg_dir: 'build/pkg',
                             depend_on: [:'bin:nugets', :ensure_key],
                             nuget_exe: 'tools/NuGet.exe',
                             api_key: ENV['NUGET_KEY']

task :default => [:'bin:compile', :'bin:nugets']

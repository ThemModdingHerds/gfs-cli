using BinaryReader = ThemModdingHerds.IO.BinaryReader;

namespace ThemModdingHerds.GFS.Client;
public static class Program
{
    public static void Main(string[] args)
    {
        if(args.Length < 1)
        {
            Console.WriteLine("Usage: ThemModdingHerds.GFS.Cli <gfs-file> [<output-folder>]");
            return;
        }
        string gfsPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory,args[0]));
        if(!System.IO.File.Exists(gfsPath))
            throw new Exception(gfsPath + " does not exist");
        string outputPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory,args.Length < 2 ?  Path.GetFileNameWithoutExtension(gfsPath) : args[1]));
        //Directory.CreateDirectory(outputPath);
        BinaryReader reader = new(gfsPath);
        File file = reader.ReadGFSFile();
        Header header = file.Header;
        List<FileEntry> entries = file.Entries;
        Console.WriteLine("id: '{0}', version: '{1}', count: {2}, offset: 0x{3}",header.Identifier,header.Version,header.EntryCount,header.DataOffset.ToString("X"));
        foreach(FileEntry entry in entries)
        {
            string filepath = Path.GetFullPath(Path.Combine(outputPath,entry.Path));
            string? dirpath = Path.GetDirectoryName(filepath);
            if(dirpath == null)
            {
                Console.WriteLine("WARNING: Invalid path found");
                continue;
            }
            if(!entry.HasData || entry.Size == 0)
            {
                Console.WriteLine("WARNING: No data for " + entry.Path);
                continue;
            }
            Directory.CreateDirectory(dirpath);
            FileStream stream = System.IO.File.Create(filepath);
            stream.Write(entry.Data);
        }
        Console.WriteLine("done");
    }
}
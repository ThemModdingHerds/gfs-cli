using ThemModdingHerds.IO.Binary;

namespace ThemModdingHerds.GFS.Client;
public static class Program
{
    public static int Main(string[] args)
    {
        if(args.Length != 1)
        {
            PrintHelp();
            return 1;
        }
        string path = args[0];
        if(File.Exists(path))
        {
            Unpack(path);
            return 0;
        }
        if(Directory.Exists(path))
        {
            Pack(path);
            return 0;
        }
        return 1;
    }
    private static void PrintHelp()
    {
        Console.WriteLine("ThemModdingHerds.GFS.CLI - .gfs (un)packer");
        Console.WriteLine();
        Console.WriteLine("usage:");
        Console.WriteLine();
        Console.WriteLine("\tThemModdingHerds.GFS.CLI.exe <gfs-file>");
        Console.WriteLine("\tThemModdingHerds.GFS.CLI.exe <folder>");
    }
    private static void Pack(string folder)
    {
        string filepath = $"{folder}.gfs";
        if(File.Exists(filepath))
            File.Delete(filepath);
        RevergePackage gfs = RevergePackage.Create(folder);
        Writer writer = new(filepath);
        writer.Write(gfs);
    }
    private static void Unpack(string file)
    {
        string output = Path.ChangeExtension(file,null);
        if(Directory.Exists(output))
            Directory.Delete(output,true);
        string metadatapath = $"{output}.metadata";
        Reader reader = new(file);
        RevergePackage gfs = reader.ReadRevergePackage();
        foreach(var pair in gfs)
        {
            RevergePackageEntry entry = pair.Value;
            string fullpath = Path.Combine(output,entry.Path);
            string? folder = Path.GetDirectoryName(fullpath);
            if(folder == null) continue;
            Directory.CreateDirectory(folder);
            Writer writer = new(fullpath);
            writer.Write(entry.Data);
            writer.Close();
        }
        if(!File.Exists(metadatapath))
            gfs.Metadata.Save(metadatapath);
    }
}
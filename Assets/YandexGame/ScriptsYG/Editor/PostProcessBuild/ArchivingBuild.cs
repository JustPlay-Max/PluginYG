using System.IO;
using System.IO.Compression;
using YG.Insides;

namespace YG.EditorScr.BuildModify
{
    public static class ArchivingBuild
    {
        public static void Archiving(string pathToBuiltProject)
        {
            InfoYG infoYG = ConfigYG.GetInfoYG();

            if (infoYG.archivingBuild)
            {
                string number = "";

                if (!File.Exists(pathToBuiltProject + ".zip"))
                {
                    Do();
                }
                else
                {
                    for (int i = 1; i < 100; i++)
                    {
                        if (!File.Exists(pathToBuiltProject + "_" + i + ".zip"))
                        {
                            number = "_" + i;
                            Do();
                            break;
                        }
                    }
                }

                void Do()
                {
                    ZipFile.CreateFromDirectory(pathToBuiltProject, pathToBuiltProject + number + ".zip");
                }
            }
        }
    }
}





using auto_designs;
using System;

class Program
{
    static void Main(string[] args)
    {
        SQLitePCL.Batteries.Init();

        FilesManager.deserializeKeywordsData();
        FilesManager.createNewDesignFiles();
    }
}

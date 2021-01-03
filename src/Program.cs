#region License

/**
 * Copyright (C) 2018 Chirstopher Torres (Raven)
 * 
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <http://www.gnu.org/licenses/>.
 */

#endregion

using System;
using System.IO;
using Revise.Files.STB;

namespace convert_tool
{
    static class Globals
    {
        public const string stbroot = "../3DDATA/stb/";
    }

    internal class Program
  {
    private struct ItemInfo
    {
      public string DataFile { get; private set; }
      public string StringFile { get; private set; }

      public ItemInfo(string dataFile, string stringFile)
      {
        this.DataFile = dataFile;
        this.StringFile = stringFile;
      }

      public void set(string dataFile, string stringFile)
      {
        this.DataFile = dataFile;
        this.StringFile = stringFile;
      }
    }

    private static void Main(string[] args)
    {
      
      const string zoneStb = "list_zone.stb";

      var itemDataFiles = new ItemInfo[14];
      itemDataFiles[00].set("list_faceitem.stb", "list_faceitem_s.stl");
      itemDataFiles[01].set("list_cap.stb", "list_cap_s.stl");
      itemDataFiles[02].set("list_body.stb", "list_body_s.stl");
      itemDataFiles[03].set("list_arms.stb", "list_arms_s.stl");
      itemDataFiles[04].set("list_foot.stb", "list_foot_s.stl");
      itemDataFiles[05].set("list_back.stb", "list_back_s.stl");
      itemDataFiles[06].set("list_jewel.stb", "list_jewel_s.stl");
      itemDataFiles[07].set("list_weapon.stb", "list_weapon_s.stl");
      itemDataFiles[08].set("list_subwpn.stb", "list_subwpn_s.stl");
      itemDataFiles[09].set("list_useitem.stb", "list_useitem_s.stl");
      itemDataFiles[10].set("list_jemitem.stb", "list_jemitem_s.stl");
      itemDataFiles[11].set("list_natural.stb", "list_natural_s.stl");
      itemDataFiles[12].set("list_questitem.stb", "list_questitem_s.stl");
      itemDataFiles[13].set("list_pat.stb", "list_pat_s.stl");

      (new FileInfo("srv_data\\scripts\\root.lua")).Directory.Create();
      var luaFile = new System.IO.StreamWriter("srv_data\\scripts\\root.lua", false);
      luaFile.Write("include(\"npc_scripts.lua\");\n");
      luaFile.Write("include(\"npc_spawns.lua\");\n");
      luaFile.Close();

      (new FileInfo("srv_data\\scripts\\npc_spawns.lua")).Directory.Create();
      luaFile = new System.IO.StreamWriter("srv_data\\scripts\\npc_spawns.lua", false);
      luaFile.Close();

      (new FileInfo("srv_data\\scripts\\npc_scripts.lua")).Directory.Create();
      luaFile = new System.IO.StreamWriter("srv_data\\scripts\\npc_scripts.lua", false);
      luaFile.Close();

      int typeIdx = 0;
      var sqlFile = new System.IO.StreamWriter("srv_data\\item_db.sql", false);
      sqlFile.Close();
      
      sqlFile = new System.IO.StreamWriter("srv_data\\skill_db.sql", false);
      sqlFile.Close();
      foreach (var itemDataFile in itemDataFiles)
      {
        var itemData = new ItemData();
        itemData.Load((ItemData.ItemType) (++typeIdx), Globals.stbroot + itemDataFile.DataFile, Globals.stbroot + itemDataFile.StringFile);
      }

      const string skillStb = "list_skill.stb";
      const string skillStl = "list_skill_s.stl";
      var skillFile = new SkillData();
      skillFile.Load(Globals.stbroot + skillStb, Globals.stbroot + skillStl);

      const string npcStb = "list_npc.stb";
      const string npcStl = "list_npc_s.stl";
      var mobFile = new MobData();
      mobFile.Load(Globals.stbroot + npcStb, Globals.stbroot + npcStl);

      var dataFile = new DataFile();
      dataFile.Load(Globals.stbroot + zoneStb);
      for (var i = 1; i < dataFile.RowCount; i++)
      {
        if (!("../"+dataFile[i][2]).Contains(".zon")) continue;

        Console.Write("Attempting to load \"" + dataFile[i][1] + "\" - ");
        var zone = new ZoneData();
        zone.Load("../" + dataFile[i][2], i);
      }

      Console.Write("Done extracting. Press any key to exit...\n");
      Console.ReadLine();
    }
  }
}
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
using Revise.Files.STB;

namespace convert_tool
{
  internal class Program
  {
    struct ItemInfo
    {
      public string DataFile { get; set; }
      public string StringFile { get; set; }

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
      const string stbRoot = "./3DDATA/stb/";
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

      foreach (var itemDataFile in itemDataFiles)
      {
        var itemData = new ItemData();
        itemData.Load(stbRoot + itemDataFile.DataFile, stbRoot + itemDataFile.StringFile);
      }

      //"list_skill.stb";
      //"list_skill_s.stl";

      var dataFile = new DataFile();
      dataFile.Load(stbRoot + zoneStb);

      Console.Write("npc(<npc_lua_file>, <map_id>, <npc_id>, <x_pos>, <y_pos>, <angle>);\n");
      Console.Write(
        "mob(<mob_spawner_alias>, <map_id>, <mob_id>, <mob_count>, <spawner_limit>, <spawner_interval>, <spawner_range>, <x_pos>, <y_pos>, <angle>);\n");
      Console.Write(
        "warp_gate(<warp_alias>, <gate_to>, <this_gate_id>, <map_id>, <x_pos>, <y_pos>, <angle>, <x_scale>, <y_scale>, <z_scale>);\n\n");

      for (var i = 1; i < dataFile.RowCount; i++)
      {
        if (!dataFile[i][2].Contains(".zon")) continue;

        Console.Write("Attempting to load \"" + dataFile[i][1] + "\" - ");
        var zone = new ZoneData();
        zone.Load(dataFile[i][2], i);
      }

      Console.ReadLine();
    }
  }
}
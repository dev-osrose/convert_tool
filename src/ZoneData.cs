// Copyright 2018 Chirstopher Torres (Raven)
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
// http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// 
// 
// \file ZoneData.cs
// \brief 
//  
// \author 
// \date 06 2018
//  

using System;
using System.Collections.Generic;
using System.IO;
using Revise.Files.IFO;
using Revise.Files.ZON;

namespace convert_tool
{
  public class ZoneData
  {
    private const int PostionModifier = 100;

    public void Load(string filePath = null, int mapId = 0)
    {
      if (filePath == null)
        return;

      string directoryPath = Path.GetDirectoryName(filePath);
      string fileName = Path.GetFileNameWithoutExtension(filePath);
      ZoneFile zoneFile = new ZoneFile();
      try
      {
        zoneFile.Load(filePath);
      }
      catch (FileNotFoundException e)
      {
        Console.Write("Failed!\n");
        return;
      }

      Console.Write("Successful!\n");

      // Do the data conversion here
      var mapDataFile = new MapDataFile[65,65];
      for (var x = 0; x < zoneFile.Width; x++)
      {
        for (var y = 0; y < zoneFile.Height; y++)
        {
          if (!zoneFile.Positions[x, y].IsUsed) continue;

          mapDataFile[x, y] = new MapDataFile();
          try
          {
            mapDataFile[x, y].Load(directoryPath + "\\" + y + "_" + x + ".ifo");
          }
          catch (FileNotFoundException e)
          {
            continue;
          }
        }
      }

      var mobList = new List<string>();
      var npcList = new List<string>();
      var warpList = new List<string>();

      foreach (var ifo in mapDataFile)
      {
        if (ifo == null) continue;

        var blockX = ifo.ZonePosition.X * PostionModifier;
        var blockY = ifo.ZonePosition.Y * PostionModifier;

        ExtractNpcs(npcList, mapId, ifo, blockX, blockY);
        ExtractMobs(mobList, mapId, ifo, blockX, blockY);
        ExtractWarpGates(warpList, mapId, ifo, blockX, blockY);
      }

      var luaFile = new System.IO.StreamWriter("scripts\\warps\\" + fileName + ".lua", false);
      using (luaFile)
      {
        luaFile.Write("--[[ WARP LIST\n");
        luaFile.Write(
          "warp_gate(<warp_alias>, <gate_to>, <map_id>, <x_pos>, <y_pos>, <angle>, <x_scale>, <y_scale>, <z_scale>);\n");
        luaFile.Write("--]]\n");
        foreach (var mapObj in warpList)
          luaFile.Write(mapObj);
      }

      luaFile = new System.IO.StreamWriter("scripts\\npcs\\" + fileName + ".lua", false);
      using (luaFile)
      {
        luaFile.WriteLine();
        luaFile.WriteLine();
        luaFile.Write("--[[ NPC SPAWN LIST\n");
        luaFile.Write("npc(<npc_lua_file>, <map_id>, <npc_id>, <x_pos>, <y_pos>, <angle>);\n");
        luaFile.Write("--]]\n");
        foreach (var mapObj in npcList)
          luaFile.Write(mapObj);
      }

      luaFile = new System.IO.StreamWriter("scripts\\mobs\\" + fileName + ".lua", false);
      using (luaFile)
      {
        luaFile.WriteLine();
        luaFile.WriteLine();
        luaFile.Write("--[[ MOB SPAWN LIST\n");
        luaFile.Write("mob(<mob_spawner_alias>, <map_id>, <mob_id>, <mob_count>, <spawner_limit>, <spawn_interval>, <spawner_range>, <x_pos>, <y_pos>);\n");
        luaFile.Write("--]]\n");
        foreach (var mapObj in mobList)
          luaFile.Write(mapObj);
      }

      Console.Write("\n\n");
    }

    private static void ExtractMobs(List<string> mobList, int mapId, MapDataFile ifo, int blockX, int blockY)
    {
      foreach (var mobSpawns in ifo.MonsterSpawns)
      {
        foreach (var normalMobs in mobSpawns.NormalSpawnPoints)
        {
          mobList.Add("mob(\"\", "
                        + mapId.ToString() + ", "
                        + normalMobs.Monster.ToString() + ", "
                        + normalMobs.Count.ToString() + ", "
                        + mobSpawns.Limit.ToString() + ", "
                        + mobSpawns.Interval.ToString() + ", "
                        + mobSpawns.Range.ToString() + ", "
                        + (blockX + mobSpawns.MapPosition.X).ToString() + ", "
                        + (blockY + mobSpawns.MapPosition.Y).ToString() + ");\n");
        }

        foreach (var tacticalMobs in mobSpawns.TacticalSpawnPoints)
        {
          mobList.Add("mob(\"\", "
                        + mapId.ToString() + ", "
                        + tacticalMobs.Monster.ToString() + ", "
                        + tacticalMobs.Count.ToString() + ", "
                        + mobSpawns.Limit.ToString() + ", "
                        + mobSpawns.Interval.ToString() + ", "
                        + mobSpawns.Range.ToString() + ", "
                        + (blockX + mobSpawns.MapPosition.X).ToString() + ", "
                        + (blockY + mobSpawns.MapPosition.Y).ToString() + ");\n");
        }
      }
    }

    private static void ExtractNpcs(List<string> npcList, int mapId, MapDataFile ifo, int blockX, int blockY)
    {
      foreach (var npc in ifo.NPCs)
      {
        npcList.Add("npc(\"\", "
                      + mapId.ToString() + ", "
                      + npc.ObjectID.ToString() + ", "
                      + (blockX + npc.MapPosition.X).ToString() + ", "
                      + (blockY + npc.MapPosition.Y).ToString() + ", "
                      + npc.Rotation.Angle + "f);\n");
      }
    }

    private static void ExtractWarpGates(List<string> warpList, int mapId, MapDataFile ifo, int blockX, int blockY)
    {
      foreach (var warpGate in ifo.WarpPoints)
      {
        warpList.Add("warp_gate(\"\", " 
                      + mapId.ToString() + ", "
                      + warpGate.WarpID.ToString() + ", "
                      + (blockX + warpGate.MapPosition.X).ToString() + ", "
                      + (blockY + warpGate.MapPosition.Y).ToString() + ", "
                      + warpGate.Rotation.Angle + "f, "
                      + warpGate.Scale.X + "f, "
                      + warpGate.Scale.Y + "f, "
                      + warpGate.Scale.Z + "f);\n");
      }
    }
  }
}
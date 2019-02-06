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
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Revise.Files.CON;
using Revise.Files.IFO;
using Revise.Files.STB;
using Revise.Files.ZMS;
using Revise.Files.ZON;
using SharpDX;

namespace convert_tool
{
  public class ZoneData
  {
    private const int PostionModifier = 100;

    public void Load(string filePath = null, int mapId = 0)
    {
      if (filePath == null)
        return;

      var directoryPath = Path.GetDirectoryName(filePath);
      var fileName = Path.GetFileNameWithoutExtension(filePath);
      var zoneFile = new ZoneFile();
      try
      {
        zoneFile.Load(filePath);
      }
      catch (FileNotFoundException)
      {
        Console.Write("Failed!\n");
        return;
      }

      Console.Write("Successful!\n");

      // Do the data conversion here
      var mapDataFile = new MapDataFile[65, 65];
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
          catch (FileNotFoundException)
          {
            continue;
          }
        }
      }

      var mobList = new List<string>();
      var npcList = new List<string>();
      var spawnList = new List<string>();
      var warpList = new List<string>();

      ExtractSpawnPoints(mapId, zoneFile, spawnList);

      foreach (var ifo in mapDataFile)
      {
        if (ifo == null) continue;

        var blockX = ifo.ZonePosition.X * PostionModifier;
        var blockY = ifo.ZonePosition.Y * PostionModifier;

        ExtractNpcs(npcList, mapId, ifo);
        ExtractMobs(mobList, mapId, ifo);
        ExtractWarpGates(warpList, mapId, ifo);
      }

      WriteLua(fileName, mobList, npcList, spawnList, warpList);

      Console.Write("\n\n");
    }

    private static void ExtractSpawnPoints(int mapId, ZoneFile zoneFile, List<string> spawnList)
    {
      const string zoneStb = "./3DDATA/stb/list_zone.stb";

      var zoneDataFile = new DataFile();
      zoneDataFile.Load(zoneStb);

      var curMapRow = zoneDataFile[mapId];

      var reviveMap = curMapRow[32];
      double.TryParse(curMapRow[33], out var reviveX);
      double.TryParse(curMapRow[34], out var reviveY);

      if (reviveMap.Length > 0)
      {
        spawnList.Add("revive_point(" + reviveMap + ", " + (reviveX * 1000.0f) + ", " + (reviveY * 10000.0f) + ");\n");
      }

      foreach (var spawnPoint in zoneFile.SpawnPoints)
      {
        if (spawnPoint.Name.Contains("WARP")) continue;

        var destCoords = new Vector3(((spawnPoint.Position.X + 520000.00f) / 100.0f), ((spawnPoint.Position.Z + 520000.00f) / 100.0f), ((spawnPoint.Position.Y) / 100.0f));

        if (spawnPoint.Name.Contains("start"))
        {
          spawnList.Add("start_point(" + mapId + ", " + destCoords.X + ", " + destCoords.Y + ");\n");
        }
        else
        {
          spawnList.Add("respawn_point(" + mapId + ", " + destCoords.X + ", " + destCoords.Y + ");\n");
        }
      }
    }

    private static void ExtractMobs(List<string> mobList, int mapId, MapDataFile ifo)
    {
      foreach (var mobSpawns in ifo.MonsterSpawns)
      {
        var adjPosCoords = new Vector3(((mobSpawns.Position.X + 520000.00f) / 100.0f), ((mobSpawns.Position.Y + 520000.00f) / 100.0f), ((mobSpawns.Position.Z) / 100.0f));
        foreach (var normalMobs in mobSpawns.NormalSpawnPoints)
        {

          mobList.Add("mob(\"\", "
                        + normalMobs.Monster.ToString() + ", "
                        + normalMobs.Count.ToString() + ", "
                        + mobSpawns.Limit.ToString() + ", "
                        + mobSpawns.Interval.ToString() + ", "
                        + mobSpawns.Range.ToString() + ", "
                        + mapId.ToString() + ", "
                        + (adjPosCoords.X).ToString() + ", "
                        + (adjPosCoords.Y).ToString() + ", "
                        + (adjPosCoords.Z).ToString() + ");\n");
        }

        foreach (var tacticalMobs in mobSpawns.TacticalSpawnPoints)
        {
          mobList.Add("mob(\"\", "
                        + tacticalMobs.Monster.ToString() + ", "
                        + tacticalMobs.Count.ToString() + ", "
                        + mobSpawns.Limit.ToString() + ", "
                        + mobSpawns.Interval.ToString() + ", "
                        + mobSpawns.Range.ToString() + ", "
                        + mapId.ToString() + ", "
                        + (adjPosCoords.X).ToString() + ", "
                        + (adjPosCoords.Y).ToString() + ", "
                        + (adjPosCoords.Z).ToString() + ");\n");
        }
      }
    }

    private static void ExtractNpcs(List<string> npcList, int mapId, MapDataFile ifo)
    {
      foreach (var npc in ifo.NPCs)
      {
        var eventDataFile = new DataFile();
        eventDataFile.Load("./3DDATA/stb/list_event.stb");
        int dialogId = 0;
        for (int i = 0; i < eventDataFile.RowCount; i++)
        {
          if (eventDataFile[i][0] == npc.ConversationFile)
          {
            dialogId = i;
            break;
          }
        }

        var adjPosCoords = new Vector3(((npc.Position.X + 520000.00f) / 100.0f), ((npc.Position.Y + 520000.00f) / 100.0f), ((npc.Position.Z) / 100.0f));
        npcList.Add("npc(\"" + dialogId + "\", "
                      + npc.ObjectID.ToString() + ", "
                      + mapId.ToString() + ", "
                      + (adjPosCoords.X).ToString() + ", "
                      + (adjPosCoords.Y).ToString() + ", "
                      + (adjPosCoords.Z).ToString() + ", "
                      + (npc.Rotation.Angle * (180.0 / Math.PI)) + ");\n");
      }
    }

    private static void ExtractWarpGates(List<string> warpList, int mapId, MapDataFile ifo)
    {
      const string warpStb = "./3DDATA/stb/warp.stb";
      const string zoneStb = "./3DDATA/stb/list_zone.stb";
      const string warpGateModel = "./3DDATA/special/warp_gate01/warp.zms";

      var zoneDataFile = new DataFile();
      zoneDataFile.Load(zoneStb);

      var warpDataFile = new DataFile();
      warpDataFile.Load(warpStb);
      var destCoords = Vector3.Zero;

      ModelFile modelFile = new ModelFile();
      modelFile.Load(warpGateModel);
      var vertices = modelFile.Vertices;

      foreach (var warpGate in ifo.WarpPoints)
      {
        var destMapId = int.Parse(warpDataFile[warpGate.WarpID][2]);
        if (zoneDataFile[destMapId][2].ToString().Contains(".zon"))
        {
          ZoneFile zoneFile = new ZoneFile();
          zoneFile.Load(zoneDataFile[destMapId][2].ToString()); // Load the zon file

          foreach (var spawnPoint in zoneFile.SpawnPoints)
          {
            if (spawnPoint.Name != warpDataFile[warpGate.WarpID][3].ToString()) continue;

            // rose is stupid and we need to do this to get the right coords
            destCoords = new Vector3(((spawnPoint.Position.X + 520000.00f) / 100.0f), ((spawnPoint.Position.Z + 520000.00f) / 100.0f), ((spawnPoint.Position.Y) / 100.0f));
            break;
          }
        }

        var position = new Vector3(((warpGate.Position.X + 520000.00f) / 100.0f), ((warpGate.Position.Y + 520000.00f) / 100.0f), ((warpGate.Position.Z) / 100.0f));

        var world = Matrix.Identity;
        var rot = Matrix.RotationQuaternion(warpGate.Rotation);
        var scale = Matrix.Scaling(warpGate.Scale);
        var trans = Matrix.Translation(position);

        var objectWorld = rot * scale * trans;

        Vector3[] vectorPositions = new Vector3[vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
          vectorPositions[i] = (Vector3)Vector3.Transform(vertices[i].Position, world * objectWorld);

        var boundingBox = BoundingBox.FromPoints(vectorPositions);

        warpList.Add("warp_gate(\"\", " 
                      + warpDataFile[warpGate.WarpID][2].ToString() + ", "
                      + (destCoords.X) + ", "
                      + (destCoords.Y) + ", "
                      + (destCoords.Z) + ", "
                      + mapId.ToString() + ", "
                      + (boundingBox.Minimum.X) + ", "
                      + (boundingBox.Minimum.Y) + ", "
                      + (boundingBox.Minimum.Z) + ", "
                      + (boundingBox.Maximum.X) + ", "
                      + (boundingBox.Maximum.Y) + ", "
                      + (boundingBox.Maximum.Z) + ");\n");
      }
    }

    private static void WriteLua(string fileName, List<string> mobList, List<string> npcList, List<string> spawnList, List<string> warpList)
    {
      var catagoryName = "fields";
      Regex townRgx = new Regex(@"^[a-zA-Z][a-zA-Z]T\d{2}$"); // matches '<2 chars>T<2 numbers>'
      if (fileName.Contains("PVP"))
      {
        catagoryName = "pvp";
      }
      else if (townRgx.IsMatch(fileName))
      {
        catagoryName = "cities";
      }

      var luaList = new List<string>();
      if (spawnList.Count > 0)
      {
        var outFilePath = "srv_data\\scripts\\spawns\\" + catagoryName + "\\" + fileName + ".lua";
        (new FileInfo(outFilePath)).Directory.Create();
        var luaFile = new System.IO.StreamWriter(outFilePath, false);
        luaList.Add("include(\"" + outFilePath + "\");\n");
        using (luaFile)
        {
          luaFile.Write("--[[ PLAYER SPAWN POINT LIST\n");
          luaFile.Write(
            "revive_point(<map_id>, <x_pos>, <y_pos>);\n");
          luaFile.Write(
            "start_point(<map_id>, <x_pos>, <y_pos>);\n");
          luaFile.Write(
            "respawn_point(<map_id>, <x_pos>, <y_pos>);\n");
          luaFile.Write("--]]\n");
          foreach (var mapObj in spawnList)
            luaFile.Write(mapObj);
        }
      }

      if (warpList.Count > 0)
      {
        var outFilePath = "srv_data\\scripts\\warps\\" + catagoryName + "\\" + fileName + ".lua";
        (new FileInfo(outFilePath)).Directory.Create();
        var luaFile = new System.IO.StreamWriter(outFilePath, false);
        luaList.Add("include(\"" + outFilePath + "\");\n");
        using (luaFile)
        {
          luaFile.Write("--[[ WARP GATE LIST\n");
          luaFile.Write(
            "warp_gate(<warp_alias>, <gate_to>, <this_gate_id>, <map_id>, <min_x_pos>, <min_y_pos>, <min_z_pos>, <max_x_pos>, <max_y_pos>, <max_z_pos>);\n");
          luaFile.Write("--]]\n");
          foreach (var mapObj in warpList)
            luaFile.Write(mapObj);
        }
      }

      if (npcList.Count > 0)
      {
        var outFilePath = "srv_data\\scripts\\npcs\\" + catagoryName + "\\" + fileName + ".lua";
        (new FileInfo(outFilePath)).Directory.Create();
        var luaFile = new System.IO.StreamWriter(outFilePath, false);
        luaList.Add("include(\"" + outFilePath + "\");\n");
        using (luaFile)
        {
          luaFile.WriteLine();
          luaFile.WriteLine();
          luaFile.Write("--[[ NPC SPAWN LIST\n");
          luaFile.Write("npc(<npc_lua_file>, <npc_id>, <map_id>, <x_pos>, <y_pos>, <z_pos>, <angle>);\n");
          luaFile.Write("--]]\n");
          foreach (var mapObj in npcList)
            luaFile.Write(mapObj);
        }
      }

      if (mobList.Count > 0)
      {
        var outFilePath = "srv_data\\scripts\\mobs\\" + catagoryName + "\\" + fileName + ".lua";
        (new FileInfo(outFilePath)).Directory.Create();
        var luaFile = new System.IO.StreamWriter(outFilePath, false);
        luaList.Add("include(\"" + outFilePath + "\");\n");
        using (luaFile)
        {
          luaFile.WriteLine();
          luaFile.WriteLine();
          luaFile.Write("--[[ MOB SPAWN LIST\n");
          luaFile.Write(
            "mob(<mob_spawner_alias>, <mob_id>, <mob_count>, <spawner_limit>, <spawn_interval>, <spawner_range>, <map_id>, <x_pos>, <y_pos>, <z_pos>);\n");
          luaFile.Write("--]]\n");
          foreach (var mapObj in mobList)
            luaFile.Write(mapObj);
        }
      }

      if (luaList.Count > 0)
      {
        var outFilePath = "srv_data\\scripts\\npc_spawns.lua";
        (new FileInfo(outFilePath)).Directory.Create();
        var luaFile = new System.IO.StreamWriter(outFilePath, true);
        using (luaFile)
        {
          foreach (var luaObj in luaList)
          {
            var luaObjFixed = luaObj.Replace("srv_data\\scripts\\", "");
            luaObjFixed = luaObjFixed.Replace("\\", "/");
            luaFile.Write(luaObjFixed);
          }
        }
      }
    }
  }
}
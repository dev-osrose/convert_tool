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
      var mapDataFile = new MapDataFile[99,99];
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

      foreach (var ifo in mapDataFile)
      {
        if (ifo == null) continue;

        var blockX = ifo.ZonePosition.X * PostionModifier;
        var blockY = ifo.ZonePosition.Y * PostionModifier;

        ExportWarpGates(mapId, ifo, blockX, blockY);
      }

      Console.Write("\n\n");
    }

    private static void ExportWarpGates(int mapId, MapDataFile ifo, int blockX, int blockY)
    {
      foreach (var warpGate in ifo.WarpPoints)
      {
        Console.Write("warp_gate(" + warpGate.WarpID.ToString() + ", " + mapId.ToString() + ", " + (blockX + warpGate.MapPosition.X).ToString() + ", " + (blockY + warpGate.MapPosition.Y).ToString() + ", " + warpGate.Rotation.Angle + "f, " + warpGate.Scale.X + "f, " + warpGate.Scale.Y + "f, " + warpGate.Scale.Z + "f);\n");
      }
    }
  }
}
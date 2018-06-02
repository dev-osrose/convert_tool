using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Revise.Files.STB;

namespace convert_tool
{
  class Program
  {
    static void Main(string[] args)
    {
      string zoneStb = "./3DDATA/stb/list_zone.stb";

      DataFile dataFile = new DataFile();
      dataFile.Load(zoneStb);

      Console.Write("npc(<npc_lua_file>, <map_id>, <npc_id>, <x_pos>, <y_pos>, <angle>);\n");
      Console.Write("mob(<mob_spawner_alias>, <map_id>, <mob_id>, <mob_count>, <spawner_limit>, <spawner_interval>, <spawner_range>, <x_pos>, <y_pos>, <angle>);\n");
      Console.Write("warp_gate(<warp_alias>, <gate_to>, <this_gate_id>, <map_id>, <x_pos>, <y_pos>, <angle>, <x_scale>, <y_scale>, <z_scale>);\n\n");

      for (var i = 1; i < dataFile.RowCount; i++)
      {
        if (!dataFile[i][2].ToString().Contains(".zon")) continue;

        Console.Write("Attempting to load \"" + dataFile[i][1].ToString() + "\" - ");
        var zone = new ZoneData();
        zone.Load(dataFile[i][2].ToString(), i);
      }

      Console.ReadLine();
    }
  }
}

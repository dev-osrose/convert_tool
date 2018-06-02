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

      //Console.Write("mob(<npc_id>, <map_id>, <x_pos>, <y_pos>, <angle>);\n");
      Console.Write("npc(<npc_id>, <map_id>, <x_pos>, <y_pos>, <angle>);\n");
      Console.Write("warp_gate(<gate_to>, <this_gate_id>, <map_id>, <x_pos>, <y_pos>, <angle>, <x_scale>, <y_scale>, <z_scale>);\n\n");

      for (int i = 1; i < dataFile.RowCount; i++)
      {
        if (dataFile[i][2].ToString().Contains(".zon"))
        {
          Console.Write("Attempting to load \"" + dataFile[i][1].ToString() + "\" - ");
          var zone = new ZoneData();
          zone.Load(dataFile[i][2].ToString(), i);
        }
      }

      Console.ReadLine();
    }
  }
}

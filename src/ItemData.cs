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
using Revise.Files.STB;
using Revise.Files.STL;
using SharpDX;

namespace convert_tool
{
  public class ItemData
  {
    public void Load(string stbPath = null, string stlPath = null)
    {
      if (stbPath == null || stlPath == null)
        return;

      var stringFile = new StringTableFile();      
      var dataFile = new DataFile();

      Console.Write("Attempting to load \"" + stbPath + "\" and \"" + stlPath + "\" - ");

      try
      {
        dataFile.Load(stbPath);
        stringFile.Load(stlPath);
      }
      catch (FileNotFoundException e)
      {
        Console.Write("Failed!\n");
        Console.WriteLine(e);
        return;
      }
      Console.Write("Successful!\n");

      for (var i = 0; i < dataFile.RowCount; i++)
      {
        StringTableRow strTableRow;
        try
        {
          strTableRow = stringFile[dataFile[i][(dataFile.ColumnCount - 1)]];
        }
        catch (ArgumentException)
        {
          continue;
        }

        var itemName = strTableRow.GetText();
        var itemDesc = strTableRow.GetDescription();

        if(itemName.Length == 0 || itemDesc.Length == 0) continue;

        string script = "";
        double priceSell = 0.0f;
        int subtype, priceBuy, weight, attack, defense, range, slots, equipJobs, groundViewModel, durability, attack_speed, magic, move_speed, usageRestrictions;
        subtype = priceBuy = weight = attack = defense = range = slots = equipJobs = groundViewModel = durability = attack_speed = magic = move_speed = usageRestrictions = 0;
        try
        {
          int.TryParse(dataFile[i][3], out usageRestrictions);
          int.TryParse(dataFile[i][4], out subtype);
          int.TryParse(dataFile[i][5], out priceBuy);
          double.TryParse(dataFile[i][6], out priceSell);
          int.TryParse(dataFile[i][7], out weight);

          int.TryParse(dataFile[i][10], out groundViewModel);
          int.TryParse(dataFile[i][16], out equipJobs);
          int.TryParse(dataFile[i][29], out durability);
          int.TryParse(dataFile[i][31], out defense);

          for (var reqIndex = 0; reqIndex < 2; reqIndex++)
          {
            var type = dataFile[i][(19 + (reqIndex * 2))];
            var value = dataFile[i][(20 + (reqIndex * 2))];

            if (type.Length == 0) continue;
          }

          for (var bonusIndex = 0; bonusIndex < 2; bonusIndex++)
          {
            var type = dataFile[i][(24 + (bonusIndex * 3))];
            var value = dataFile[i][(25 + (bonusIndex * 3))];

            if (type.Length == 0) continue;
          }
        }
        catch (ArgumentOutOfRangeException e)
        {
          Console.WriteLine(e);
          continue;
        }
      }
    }
  }
}
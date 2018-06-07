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
    enum ItemType : int {
      FACE = 1,
      CAP,
      BODY,
      ARMS,
      FOOT,
      BACK,
      JEWEL,
      WEAPON,
      SUB_WEAPON,
      USEITEM,
      JEMITEM,
      NATURAL,
      QUESTITEM,
      PAT,
      MAX_ITEM_TYPES
    };

    public void Load(int type = 0, string stbPath = null, string stlPath = null)
    {
      if (type == 0 || stbPath == null || stlPath == null)
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
        var curRow = dataFile[i];
        try
        {
          strTableRow = stringFile[curRow[(dataFile.ColumnCount - 1)]];
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
        int subtype, priceBuy, weight, attack, defense, range, slots, equipJobs, groundViewModel, durability, attackSpeed, magic, moveSpeed, usageRestrictions;
        subtype = priceBuy = weight = attack = defense = range = slots = equipJobs = groundViewModel = durability = attackSpeed = magic = moveSpeed = usageRestrictions = 0;
        try
        {
          int.TryParse(curRow[3], out usageRestrictions);
          int.TryParse(curRow[4], out subtype);
          int.TryParse(curRow[5], out priceBuy);
          double.TryParse(curRow[6], out priceSell);
          int.TryParse(curRow[7], out weight);

          int.TryParse(curRow[10], out groundViewModel);
          int.TryParse(curRow[16], out equipJobs);

          for (var reqIndex = 0; reqIndex < 2; reqIndex++)
          {
            var type = curRow[(19 + (reqIndex * 2))];
            var value = curRow[(20 + (reqIndex * 2))];

            if (type.Length == 0) continue;
          }

          for (var bonusIndex = 0; bonusIndex < 2; bonusIndex++)
          {
            var type = curRow[(24 + (bonusIndex * 3))];
            var value = curRow[(25 + (bonusIndex * 3))];

            if (type.Length == 0) continue;
          }

          int.TryParse(curRow[29], out durability);
          int.TryParse(curRow[31], out defense);

          switch (type)
          {
            case ItemType::BACK:
            case ItemType::FOOT:
            {
              int.TryParse(curRow[33], out moveSpeed);
              break;
            }
            case ItemType::WEAPON: 
            {
              int.TryParse(curRow[30], out slots);
              int.TryParse(curRow[33], out range);
              int.TryParse(curRow[35], out attack);
              int.TryParse(curRow[36], out attackSpeed);
              int.TryParse(curRow[37], out magic);
              break;
            }
            case ItemType::SUB_WEAPON: 
            {
              int.TryParse(curRow[30], out slots);
              break;
            }
            default:
              break;
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
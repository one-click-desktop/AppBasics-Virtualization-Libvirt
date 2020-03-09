/*
 * Libvirt-dotnet
 * 
 * Copyright 2020 IDNT (https://www.idnt.net) and Libvirt-dotnet contributors.
 * 
 * This project incorporates work by the following original authors and contributors
 * to libvirt-csharp:
 *    
 *    Copyright (C) 
 *      Arnaud Champion <arnaud.champion@devatom.fr>
 *      Jaromír Červenka <cervajz@cervajz.com>
 *
 * Licensed under the GNU Lesser General Public Library, Version 2.1 (the "License");
 * you may not use this file except in compliance with the License. You may obtain a 
 * copy of the License at
 *
 * https://www.gnu.org/licenses/lgpl-2.1.en.html
 * 
 * or see LICENSE for a copy of the license terms. Unless required by applicable 
 * law or agreed to in writing, software distributed under the License is distributed 
 * on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express 
 * or implied. See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;

namespace Libvirt
{
    internal class GraphicsHelper
    {
        static public Bitmap PortablePixmapToBitmap(Stream stream, Color? noVideoColor = null)
        {
            bool isAllBlack = noVideoColor.HasValue;
            var nc = noVideoColor.HasValue ? noVideoColor.Value.R + noVideoColor.Value.G + noVideoColor.Value.B : 0;

            using (var reader = new BinaryReader(stream))
            {
                if (reader.ReadChar() != 'P' || reader.ReadChar() != '6')
                    return null;

                reader.ReadChar();
                string widths = "", heights = "";

                char temp;
                while ((temp = reader.ReadChar()) != ' ')
                    widths += temp;

                while ((temp = reader.ReadChar()) >= '0' && temp <= '9')
                    heights += temp;

                if (reader.ReadChar() != '2' || reader.ReadChar() != '5' || reader.ReadChar() != '5')
                    return null;

                reader.ReadChar();
                int width = int.Parse(widths),
                    height = int.Parse(heights);
                Bitmap bitmap = new Bitmap(width, height);

                Color c;
                for (int y = 0; y < height; y++)
                    for (int x = 0; x < width; x++)
                    {
                        bitmap.SetPixel(x, y, (c = Color.FromArgb(reader.ReadByte(), reader.ReadByte(), reader.ReadByte())));
                        if (isAllBlack && (c.R + c.G + c.B) != nc)
                            isAllBlack = false;
                    }

                if (isAllBlack)
                {
                    bitmap.Dispose();
                    return null;
                }
                return bitmap;
            }
        }
    }
}

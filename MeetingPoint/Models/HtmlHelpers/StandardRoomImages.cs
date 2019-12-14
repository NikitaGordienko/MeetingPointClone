using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace MeetingPoint.Models.HtmlHelpers
{
    public static class StandardRoomImages
    {
        public static HtmlString CreateStandardRoomImagesList(this IHtmlHelper html)
        {
            // Количество картинок
            int count = 6;

            TagBuilder mainDiv = new TagBuilder("div");
            mainDiv.AddCssClass("default_room_images");

            TagBuilder imagesLine = new TagBuilder("div");
            imagesLine.AddCssClass("room_images_line");

            for (int i = 1; i <= count; i++)
            {
                TagBuilder img = new TagBuilder("img");
                img.Attributes.Add("src", "/images/room_avatar/r_" + i + ".png");
                img.Attributes.Add("alt", "r_" + i);
                img.Attributes.Add("id", "r_" + i);
                img.AddCssClass("room_create_image_min");
                imagesLine.InnerHtml.AppendHtml(img);
            }

            mainDiv.InnerHtml.AppendHtml(imagesLine);

            var writer = new StringWriter();

            mainDiv.WriteTo(writer, HtmlEncoder.Default);

            return new HtmlString(writer.ToString());
        }
    }
}

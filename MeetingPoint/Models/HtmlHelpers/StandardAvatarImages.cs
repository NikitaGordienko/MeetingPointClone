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
    public static class StandardAvatarImages
    {
        public static HtmlString CreateStandardAvatarImagesList(this IHtmlHelper html)
        {
            // Количество картинок (всегда должно быть count/2 картинок с префиксом 'm' и префиксом 'w'
            int count = 12;


            // TODO: Произвести рефакторинг кода
            TagBuilder mainDiv = new TagBuilder("div");
            mainDiv.AddCssClass("default_avatar_images");

            TagBuilder div1 = new TagBuilder("div");
            div1.AddCssClass("avatar_images_line");

            TagBuilder div2 = new TagBuilder("div");
            div2.AddCssClass("avatar_images_line");



            for (int i = 1; i <= count / 2; i++)
            {
                TagBuilder img1 = new TagBuilder("img");
                img1.Attributes.Add("src", "/images/user_avatar/m_" + i + ".png");
                img1.Attributes.Add("alt", "m_" + i);
                img1.Attributes.Add("id", "m_" + i);
                img1.AddCssClass("avatar_registration_min");
                div1.InnerHtml.AppendHtml(img1);

                TagBuilder img2 = new TagBuilder("img");
                img2.Attributes.Add("src", "/images/user_avatar/w_" + i + ".png");
                img2.Attributes.Add("alt", "w_" + i);
                img2.Attributes.Add("id", "w_" + i);
                img2.AddCssClass("avatar_registration_min");
                div2.InnerHtml.AppendHtml(img2);
            }

            mainDiv.InnerHtml.AppendHtml(div1);
            mainDiv.InnerHtml.AppendHtml(div2);

            var writer = new StringWriter();
            mainDiv.WriteTo(writer, HtmlEncoder.Default);

            return new HtmlString(writer.ToString());
        }
    }
}

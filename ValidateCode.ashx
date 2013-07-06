<%@ WebHandler Language="C#" Class="ValidateCode" %>

using System;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Web.SessionState;

public class ValidateCode : IHttpHandler, IRequiresSessionState
{
    public void ProcessRequest(HttpContext context)
    {
        CreateCheckCodeImage(GenerateCheckCode(context), context);
    }
    
    private string GenerateCheckCode(HttpContext context)
    {
        int number;
        char code;
        string checkCode = String.Empty;
        System.Random random = new Random();
        for (int i = 0; i < 5; i++)
        {
            number = random.Next();
            if (number % 2 == 0)
                code = (char)('0' + (char)(number % 10));
            else
                code = (char)('A' + (char)(number % 26));
            checkCode += code.ToString();
        }
        //儲存在cookie
        // context.Response.Cookies.Add(new HttpCookie("CheckCode", checkCode));
        
        //儲存在session
        context.Session["CheckCode"] = checkCode;

        return checkCode;
    }
    
    private void CreateCheckCodeImage(string checkCode, HttpContext context)
    {
        if (checkCode == null || checkCode.Trim() == String.Empty)
            return;
        
        System.Drawing.Bitmap image = new System.Drawing.Bitmap((int)Math.Ceiling((checkCode.Length * 12.5)), 22);
        Graphics g = Graphics.FromImage(image);
        try
        {
            //產生亂數
            Random random = new Random();
            //背景色
            g.Clear(Color.White);
            Font font = new System.Drawing.Font("Arial", 12, (System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Italic));
            System.Drawing.Drawing2D.LinearGradientBrush
            brush = new System.Drawing.Drawing2D.
            LinearGradientBrush(new Rectangle(0, 0, image.Width, image.Height), Color.Black, Color.Black, 1.2f, true);
            g.DrawString(checkCode, font, brush, 2, 2);
            //圖片邊框
            g.DrawRectangle(new Pen(Color.Black),
            0, 0, image.Width - 1, image.Height - 1);
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            image.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            context.Response.ClearContent();
            context.Response.ContentType = "image/Gif";
            context.Response.BinaryWrite(ms.ToArray());
        }
        finally
        {
            g.Dispose();
            image.Dispose();
        }
    }
    public bool IsReusable
    {
        get
        { return false; }
    }

}
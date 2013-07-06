using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Security;

public partial class _Default : System.Web.UI.Page
{
    private bool IsCreateSuc;  

    protected void Page_Load(object sender, EventArgs e)
    {
        // Get label from CreateUserWizard control.
        Control ctrllbl = CreateUserWizardName.CreateUserStep.ContentTemplateContainer.FindControl("lbMessage");
        if (ctrllbl != null)
        {
            if (!IsPostBack)
            {
                Label lbl = (Label)ctrllbl;
                string captchaCode = GenerateRandomCode();
                this.Session["CaptchaImageText"] = captchaCode;
                lbl.Text = captchaCode;
             }
         }

    }

    protected void CreateUserWizardName_CreatingUser(object sender, LoginCancelEventArgs e)
    {
        Control ctrl = CreateUserWizardName.CreateUserStep.ContentTemplateContainer.FindControl("codeNumberTextBox");
        TextBox txb = (TextBox)ctrl;
        // You can check the user input like this: 
        if (txb.Text == this.Session["CheckCode"].ToString())    
        {
            this.IsCreateSuc = true;
        }
        else
        {
            this.IsCreateSuc = false;
            lbError.Text = "ERROR: Captcha code incorrect, please try again.";
            txb.Text = "";
            e.Cancel = true;
        }
    } 

    protected void CreateUserWizardName_NextButtonClick(object sender, WizardNavigationEventArgs e)
    {
        if (IsCreateSuc)
        {
            // Display an informational message
            lbError.Text = "Create a new user";
            // Display your information
            Control ctrlName = CreateUserWizardName.CreateUserStep.ContentTemplateContainer.FindControl("UserName");
            string strUserName = ((TextBox)ctrlName).Text;
            Control ctrlPassword = CreateUserWizardName.CreateUserStep.ContentTemplateContainer.FindControl("Password");
            string strPassword = ((TextBox)ctrlPassword).Text;
            Control ctrlLbUsername = CreateUserWizardName.CompleteStep.ContentTemplateContainer.FindControl("lbUserName");
            Control ctrlLbEmail = CreateUserWizardName.CompleteStep.ContentTemplateContainer.FindControl("lbEmail");
            Control ctrlLbQuestion = CreateUserWizardName.CompleteStep.ContentTemplateContainer.FindControl("lbQuestion");
            MembershipUser userInfo;
            if (Membership.ValidateUser(strUserName, strPassword) == true)
            {
                userInfo = Membership.GetUser(strUserName);
                ((Label)ctrlLbUsername).Text = userInfo.UserName;
                ((Label)ctrlLbQuestion).Text = userInfo.PasswordQuestion;
                ((Label)ctrlLbEmail).Text = userInfo.Email;
            }
        }
        else
        {
            e.Cancel = true;
        }
    }

            // Used to create numbers randomly 
        private string GenerateRandomCode()
        {
            string allChar = "1,2,3,4,5,6,7,8,9,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z";
            int codeCount = 6;
            string[] allCharArray = allChar.Split(',');
            string randomCode = "";
            int temp = -1;

            Random rand = new Random();
            for (int i = 0; i < codeCount; i++)
            {
                if (temp != -1)
                {
                    rand = new Random(i * temp * ((int)DateTime.Now.Ticks));
                }
                int t = rand.Next(allCharArray.Length - 1);
                if (temp == t)
                {
                    return GenerateRandomCode();
                }
                temp = t;
                randomCode += allCharArray[t];
            }

            // Set value to session variable when the function is called. 
            Session["CaptchaImageText"] = randomCode;
            return randomCode;
        }
}
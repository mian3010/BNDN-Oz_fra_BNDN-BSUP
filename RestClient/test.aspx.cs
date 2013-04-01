using System;
using System.Collections.Generic;

namespace RestClient {
  public partial class test : System.Web.UI.Page {
    protected void Page_Load(object sender, EventArgs e) {

    }

    protected void Auth_Click(object sender, EventArgs e) {
      var c = new RentItService();
      try {
        var x = c.Authenticate("Lynette", "Awesome");
        AuthResponse.Text = x;
      }
      catch (Exception es) {
        AuthResponse.Text = es.ToString();
      }
    }
    protected void Update_Click(object sender, EventArgs e) {
      var c = new RentItService();
      try {
        var data = new Dictionary<string, object> { { "email", "testupdate@example.com" } };
        var x = c.UpdateAccount("Lynette", data, Token.Text);
        UpdateResponse.Text = x;
      }
      catch (Exception es) {
        UpdateResponse.Text = es.ToString();
      }
    }
    protected void Create_Click(object sender, EventArgs e) {
      var c = new RentItService();
      try {
        var data = new Dictionary<string, object> {
          {"password", "MichaelPass" },
          {"email", "michael@example.com" },
          {"address", "Care" },
          {"postal", "12" },
          {"country", "Denmark" },
          {"accountType", "Customer" }
        };
        var x = c.CreateAccount("Michael", data);
        CreateResponse.Text = x;
      }
      catch (Exception es) {
        CreateResponse.Text = es.ToString();
      }
    }
    protected void GetAccount_Click(object sender, EventArgs e) {
      var c = new RentItService();
      try {
        var x = c.GetAccount("Lynette");
        GetAccountResponse.Text = x;
      }
      catch (Exception es) {
        GetAccountResponse.Text = es.ToString();
      }
    }
    protected void GetAccounts_Click(object sender, EventArgs e) {
      var c = new RentItService();
      try {
        var x = c.GetAccounts("ACP", "detailed", "true");
        GetAccountsResponse.Text = x;
      }
      catch (Exception es) {
        GetAccountsResponse.Text = es.ToString();
      }
    }
  }
}
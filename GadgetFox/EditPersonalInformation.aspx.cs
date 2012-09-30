﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data.SqlClient;

namespace GadgetFox
{
    public partial class EditPersonalInformation : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Session["userID"] != null)
                {
                    String myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
                    SqlConnection myConnection = new SqlConnection(myConnectionString);
                    try
                    {
                        myConnection.Open();
                        SqlCommand cmd = new SqlCommand("Select * from [GadgetFox].[dbo].[Users] where EmailID=@EmailID", myConnection);
                        cmd.Parameters.AddWithValue("@EmailID", Session["userID"].ToString());
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.Read())
                        {
                            firstNameTB.Text = dr["FirstName"].ToString();
                            lastNameTB.Text = dr["LastName"].ToString();
                            if (dr["DOB"] != null)
                                birthDateTB.Text = ((System.DateTime)dr["DOB"]).ToString("dd/MM/yyyy");
                            if (dr["PhoneNum"] != null)
                                phoneNumberTB.Text = dr["PhoneNum"].ToString();
                        }
                    }
                    catch (SqlException ex)
                    {
                        Response.Write("<SCRIPT LANGUAGE='JavaScript'>alert('" + ex.Message + "')</SCRIPT>");
                    }
                    finally
                    {
                        myConnection.Close();
                    }
                }
            }

        }

        protected void saveButton_Click(object sender, EventArgs e)
        {
            String myConnectionString = ConfigurationManager.ConnectionStrings["myConnectionString"].ConnectionString;
            SqlConnection myConnection = new SqlConnection(myConnectionString);
            try
            {
                myConnection.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO [GadgetFox].[dbo].[Users] ([FirstName],[LastName],[DOB],[PhoneNum])" +
                "VALUES(@FirstName,@LastName,@DOB,@PhoneNum)", myConnection);
                cmd.Parameters.AddWithValue("@FirstName", firstNameTB.Text);
                cmd.Parameters.AddWithValue("@LastName", lastNameTB.Text);
                if (!String.IsNullOrEmpty(birthDateTB.Text) || !birthDateTB.Text.Equals("MM/DD/YYYY"))
                    cmd.Parameters.AddWithValue("@DOB", Convert.ToDateTime(birthDateTB.Text));
                else
                    cmd.Parameters.AddWithValue("@DOB", null);
                cmd.Parameters.AddWithValue("@PhoneNum", phoneNumberTB.Text);
                int rows = cmd.ExecuteNonQuery();
                if (rows == 1)
                {
                    Response.Write("<SCRIPT LANGUAGE='JavaScript'>alert('Saved Information successfully')</SCRIPT>");
                    Response.Redirect("~/Home.aspx");
                }
            }
            catch (SqlException ex)
            {
                Response.Write("<SCRIPT LANGUAGE='JavaScript'>alert('" + ex.Message + "')</SCRIPT>");
            }
            finally
            {
                myConnection.Close();
            }
        }
    }
}
    


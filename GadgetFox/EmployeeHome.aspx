<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite2.master" AutoEventWireup="true" CodeBehind="EmployeeHome.aspx.cs" Inherits="GadgetFox.Home" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <!-- CSS style should be in stylesheet (eventually) -->
    <table>
        <tr>
            <td style="display: inline-block; height: 250px; width: 250px;">
                <a href="SalesDashboard.aspx"><label style="display: block; font-size: 18px; font-weight: bold; text-align: center; color: #383838;">Sales & Marketing</label><image src="images/sales_marketing.png" style="display: block; height: 60%; margin: auto;"/></a>
            </td>
            <td style="display: inline-block; height: 250px; width: 250px;">
                <a href="ViewCustomerIssues.aspx"><label style="display: block; font-size: 18px; font-weight: bold; text-align: center; color: #383838;">Customer Service</label><image src="images/customer_service.png" style="display: block; height: 60%; margin: auto;"/></a>
            </td>
            <td style="display: inline-block; height: 250px; width: 250px;">
                <a href="ViewInventory.aspx"><label style="display: block; font-size: 18px; font-weight: bold; text-align: center; color: #383838;">Warehouse</label><image src="images/warehouse.png" style="display: block; height: 60%; margin: auto;"/></a>
            </td>
        </tr>
    </table>
</asp:Content>

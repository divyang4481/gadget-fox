<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite2.master" AutoEventWireup="true" CodeBehind="Forbidden.aspx.cs" Inherits="GadgetFox.Forbidden" %>

<asp:Content ID="Content1" ContentPlaceHolderID="headContent" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:Panel ID="pnlForbidden" runat="server" Width="60%">
        <fieldset>
            <legend class="forbidden">Forbidden</legend>
            <asp:Label ID="lblForbidden" runat="server" CssClass="forbiddenLabel" Text="You are forbidden to run this command"></asp:Label>
        </fieldset>
    </asp:Panel>

</asp:Content>

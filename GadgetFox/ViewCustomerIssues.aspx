<%@ Page Title="" Language="C#" MasterPageFile="~/GadgetSite2.master" AutoEventWireup="true" CodeBehind="ViewCustomerIssues.aspx.cs" Inherits="GadgetFox.ViewCustomerIssues" %>

<asp:Content ID="Content1" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div class="inventoryGrid">
         <asp:GridView ID="customerIssues" runat="server" CellPadding="4" ForeColor="Black" GridLines="Vertical" BackColor="White" BorderColor="#DEDFDE" BorderStyle="None" BorderWidth="1px" AutoGenerateColumns="False" 
            AllowSorting="True" AllowPaging="True" DataKeyNames="IssueID" DataSourceID="SqlDataSource1">
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:CommandField ButtonType="Button" ShowEditButton="True" />
                <asp:BoundField DataField="EmailID" HeaderText="Email ID" ReadOnly="True" SortExpression="EmailID" />
                <asp:BoundField DataField="IssueID" HeaderText="Issue ID" ReadOnly="True" SortExpression="IssueID" />
                <asp:BoundField DataField="IssueType" HeaderText="Type" ReadOnly="True" SortExpression="Type" />
                <asp:BoundField DataField="IssueDescription" HeaderText="Description" />
                <asp:BoundField DataField="AssignedTo" HeaderText="Assigned To" SortExpression="AssignedTo" />
                <asp:TemplateField HeaderText="Status" SortExpression="Status">
                    <ItemTemplate>
                        <asp:DropDownList ID="statusDropDown" AutoPostBack="true" DataTextField="Status" DataValueField="Status" runat="server" SelectedValue='<%# Bind("Status") %>'>
                            <asp:ListItem>Select</asp:ListItem>
                            <asp:ListItem>Pending</asp:ListItem>
                            <asp:ListItem>Resolved</asp:ListItem>
                        </asp:DropDownList>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:BoundField DataField="OrderID" HeaderText="Order ID" ReadOnly="True" SortExpression="OrderID" />
            </Columns>
            <EmptyDataTemplate>
                <asp:Label ID="lblNoData" runat="server" Text="No customer issues"></asp:Label>
            </EmptyDataTemplate>
            <FooterStyle BackColor="#CCCC99" />
            <HeaderStyle BackColor="#6B696B" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#F7F7DE" ForeColor="Black" HorizontalAlign="Right" />
            <RowStyle BackColor="#F7F7DE" />
            <SelectedRowStyle BackColor="#CE5D5A" Font-Bold="True" ForeColor="White" />
            <SortedAscendingCellStyle BackColor="#FBFBF2" />
            <SortedAscendingHeaderStyle BackColor="#848384" />
            <SortedDescendingCellStyle BackColor="#EAEAD3" />
            <SortedDescendingHeaderStyle BackColor="#575357" />
        </asp:GridView>
        <asp:SqlDataSource ID="SqlDataSource1" runat="server" ConnectionString='<%$ ConnectionStrings:myConnectionString %>' 
            SelectCommand="SELECT * FROM [Issues]" 
            UpdateCommand="UPDATE [GadgetFox].[dbo].[Issues] SET [IssueDescription]=@IssueDescription, [AssignedTo]=@AssignedTo, [Status]=@Status WHERE IssueID=@IssueID">
            <UpdateParameters>
                <asp:Parameter Type="String" name="IssueDescription"/>
                <asp:Parameter Type="String" name="AssignedTo"/>
                <asp:Parameter Type="String" name="Status"/>
            </UpdateParameters>
        </asp:SqlDataSource>
    </div>
    <div>
        <asp:Button ID="buttonReportIssue" runat="server" OnClick="buttonReportIssue_Click" Text="Report issue" PostBackUrl="~/ReportIssue.aspx" Width="140px" style="height: 25px; margin-left: 80px; margin-top: 10px;" />
    </div>
</asp:Content>

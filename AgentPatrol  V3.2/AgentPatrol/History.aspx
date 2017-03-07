<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="History.aspx.cs" Inherits="History" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>

<asp:Content ID="Content" ContentPlaceHolderID="body" runat="Server">
    <form runat="server">
        <asp:ScriptManager EnablePartialRendering="true" EnablePageMethods="true"
            ID="ScriptManager1" runat="server">
        </asp:ScriptManager>


        <asp:Button runat="server" ID="btnReturn" Text="Back To Maps" OnClick="btnReturn_Click" UseSubmitBehavior="false" CssClass="button" />

        <br />

        <div class="center">
            <asp:UpdatePanel runat="server" ID="UDPContained" UpdateMode="always">
                <ContentTemplate>
                    <asp:TextBox runat="server" ID="txtSearch" />
                    <asp:Button runat="server" ID="btnSearch" Text="Search" CssClass="button" />

                    <asp:GridView ID="gvRuns" runat="server" AutoGenerateColumns="false" DataKeyNames="ID" AllowPaging="true"
                        AllowSorting="true" OnRowCommand="gvRuns_RowCommand" DataSourceID="sqlRuns">
                        <PagerStyle CssClass="pager-row" />
                        <EditRowStyle CssClass="edit-row" />
                        <SortedAscendingCellStyle CssClass="Asc_Des_Cell" />
                        <SortedAscendingHeaderStyle CssClass="Asc_Des_Header" />
                        <SortedDescendingCellStyle CssClass="Asc_Des_Cell" />
                        <SortedDescendingHeaderStyle CssClass="Asc_Des_Header" />
                        <Columns>
                            <asp:BoundField DataField="ID" HeaderText="ID" SortExpression="ID" />
                            <asp:BoundField DataField="fileName" HeaderText="File Name" SortExpression="fileName" />
                            <asp:BoundField DataField="timeStamp" HeaderText="Time Stamp" SortExpression="timeStamp" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:Button ID="btnView" runat="server" CommandName="ViewRunHistory" Text="View Run Data" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            No Files Match This Search
                        </EmptyDataTemplate>
                    </asp:GridView>
                    <asp:SqlDataSource ID="sqlRuns" runat="server" SelectCommandType="StoredProcedure" SelectCommand="getOldRuns"
                        FilterExpression="FileName LIKE '%{0}%'">
                        <FilterParameters>
                            <asp:ControlParameter Name="FileName" ControlID="txtSearch" PropertyName="Text" />
                        </FilterParameters>
                    </asp:SqlDataSource>
                    <br />
                    <br />
                    <asp:TextBox runat="server" ID="txtResults" TextMode="MultiLine" />
                    <br />
                    <br />
                    <br />
                    <br />
                    <br />
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="btnSearch" />
                </Triggers>
            </asp:UpdatePanel>
        </div>
    </form>
</asp:Content>

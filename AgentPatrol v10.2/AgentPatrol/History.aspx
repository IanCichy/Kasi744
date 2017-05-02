<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="History.aspx.cs" Inherits="History" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>

<asp:Content ID="Content" ContentPlaceHolderID="body" runat="Server">
    <form runat="server" class="form">
        <asp:ScriptManager EnablePartialRendering="true" EnablePageMethods="true"
            ID="ScriptManager" runat="server">
        </asp:ScriptManager>
        <asp:Button runat="server" ID="doNothingButton" OnClientClick="return false;" CssClass="hidden" />
        <asp:Button runat="server" ID="btnReturn" Text="Back To Maps" OnClick="btnReturn_Click" CssClass="button-warning" />
        <br />
        <br />
        <asp:UpdatePanel runat="server" ID="UDPContained" UpdateMode="always">
            <ContentTemplate>
                <div class="left-half">
                    <asp:GridView ID="gvRuns" runat="server" AutoGenerateColumns="false"
                        DataKeyNames="ID" AllowPaging="true"
                        AllowSorting="true" OnRowCommand="gvRuns_RowCommand"
                        DataSourceID="sqlRuns" OnRowCreated="gvRuns_RowCreated"
                        CssClass="gridView">
                        <RowStyle CssClass="row-gv" />
                        <AlternatingRowStyle CssClass="alt-row-gv" />
                        <PagerStyle CssClass="pager-row" />
                        <HeaderStyle CssClass="header-gv" />
                        <EditRowStyle CssClass="edit-row" />
                        <SortedAscendingCellStyle CssClass="Asc_Des_Cell" />
                        <SortedAscendingHeaderStyle CssClass="Asc_Des_Header" />
                        <SortedDescendingCellStyle CssClass="Asc_Des_Cell" />
                        <SortedDescendingHeaderStyle CssClass="Asc_Des_Header" />
                        <Columns>
                            <asp:BoundField DataField="fileName" HeaderText="File Name" SortExpression="fileName" />
                            <asp:BoundField DataField="timeStamp" HeaderText="Time" SortExpression="timeStamp" />
                            <asp:BoundField DataField="mapSize" HeaderText="Map Size" SortExpression="mapSize" />
                            <asp:BoundField DataField="regions" HeaderText="Regions" SortExpression="regions" />
                            <asp:BoundField DataField="agents" HeaderText="Agents" SortExpression="agents" />
                            <asp:BoundField DataField="stepsTaken" HeaderText="Steps" SortExpression="stepsTaken" />
                            <asp:BoundField DataField="stepLimit" HeaderText="Step Limit" SortExpression="stepLimit" />
                            <asp:BoundField DataField="algorithm" HeaderText="Algorithm" SortExpression="algorithm" />
                            <asp:TemplateField>
                                <ItemTemplate>
                                    <asp:LinkButton ID="btnView" runat="server" CommandName="ViewRunHistory" Text="View Data" CssClass="linkbutton-Gridview" />
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                        <EmptyDataTemplate>
                            No Files Match This Search
                        </EmptyDataTemplate>
                    </asp:GridView>
                    <asp:SqlDataSource ID="sqlRuns" runat="server" SelectCommandType="StoredProcedure" SelectCommand="getOldRuns"
                        FilterExpression="FileName LIKE '%{0}%' AND MapSize LIKE '%{2}%' AND Regions LIKE '%{3}%' AND Agents LIKE '%{4}%' AND StepsTaken LIKE '%{5}%' AND StepLimit LIKE '%{6}%' AND Algorithm LIKE '%{7}%' AND timeStamp >= '{8}' AND timeStamp <= '{9}'" >
                        <FilterParameters>
                            <asp:ControlParameter Name="FileName" ControlID="txtFileName"
                                PropertyName="Text" Type="String" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:ControlParameter Name="DateTime" ControlID="txtFileName"
                                PropertyName="Text" Type="String" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:ControlParameter Name="MapSize" ControlID="txtMapSize"
                                PropertyName="Text" Type="String" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:ControlParameter Name="Regions" ControlID="txtRegions"
                                PropertyName="Text" Type="String" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:ControlParameter Name="Agents" ControlID="txtAgents"
                                PropertyName="Text" Type="String" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:ControlParameter Name="StepsTaken" ControlID="txtSteps"
                                PropertyName="Text" Type="String" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:ControlParameter Name="StepLimit" ControlID="txtLimit"
                                PropertyName="Text" Type="String" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:ControlParameter Name="Algorithm" ControlID="ddlAlgorithm"
                                PropertyName="Text" Type="String" DefaultValue="" ConvertEmptyStringToNull="false" />
                            <asp:ControlParameter Name="Date1" ControlID="txtDate1"
                                PropertyName="Text" Type="dateTime" DefaultValue="4/1/2017" ConvertEmptyStringToNull="true" />
                            <asp:ControlParameter Name="Date2" ControlID="txtDate2"
                                PropertyName="Text" Type="dateTime" DefaultValue="4/1/2099" ConvertEmptyStringToNull="true" />
                        </FilterParameters>
                    </asp:SqlDataSource>
                    <br />
                    <asp:TextBox runat="server" ID="txtResults" TextMode="MultiLine" />
                </div>
                <div class="right-half">
                    <asp:Label runat="server" ID="lblFileName" Text="File Name:" CssClass="labelSearch" />
                    <br />
                    <asp:TextBox runat="server" ID="txtFileName" CssClass="containerSearch" placeholder="..." AutoPostBack="false" />
                    <br />
                    <!-- -->
                    <asp:Label runat="server" ID="lblMapSize" Text="Map Size:" CssClass="labelSearch" /><br />
                    <asp:TextBox runat="server" ID="txtMapSize" CssClass="containerSearch" placeholder="..." AutoPostBack="false" />
                    <br />
                    <!-- -->
                    <asp:Label runat="server" ID="lblRegions" Text="# of Regions:" CssClass="labelSearch" /><br />
                    <asp:TextBox runat="server" ID="txtRegions" CssClass="containerSearch" placeholder="..." AutoPostBack="false" />
                    <br />
                    <!-- -->
                    <asp:Label runat="server" ID="lblAgents" Text="# of Agents:" CssClass="labelSearch" /><br />
                    <asp:TextBox runat="server" ID="txtAgents" CssClass="containerSearch" placeholder="..." AutoPostBack="false" />
                    <br />
                    <!-- -->
                    <asp:Label runat="server" ID="lblSteps" Text="Total Steps:" CssClass="labelSearch" /><br />
                    <asp:TextBox runat="server" ID="txtSteps" CssClass="containerSearch" placeholder="..." AutoPostBack="false" />
                    <br />
                    <!-- -->
                    <asp:Label runat="server" ID="lblLimit" Text="Step Limit:" CssClass="labelSearch" /><br />
                    <asp:TextBox runat="server" ID="txtLimit" CssClass="containerSearch" placeholder="..." AutoPostBack="false" />
                    <br />
                    <!-- -->
                    <asp:Label runat="server" ID="lblAlgorithm" Text="Algorithm:" CssClass="labelSearch" /><br />
                    <asp:DropDownList runat="server" ID="ddlAlgorithm" CssClass="containerSearch" AutoPostBack="false">
                        <asp:ListItem Selected="True" Value=""> ... </asp:ListItem>
                        <asp:ListItem Value="Free Form"> Free Form </asp:ListItem>
                        <asp:ListItem Value="Constrained-3"> Constrained-3 </asp:ListItem>
                        <asp:ListItem Value="Constrained-4"> Constrained-4 </asp:ListItem>
                    </asp:DropDownList>
                    <br />
                    <!-- -->
                    <asp:Label runat="server" ID="lblDate1" Text="Start Date:" CssClass="labelSearch" /><br />
                    <asp:TextBox runat="server" ID="txtDate1" CssClass="containerSearch" TextMode="DateTime" AutoPostBack="false" />
                    <br />
                    <!-- -->
                    <asp:Label runat="server" ID="lblDate2" Text="End Date:" CssClass="labelSearch" /><br />
                    <asp:TextBox runat="server" ID="txtDate2" CssClass="containerSearch" TextMode="DateTime" AutoPostBack="false" />
                    <br />
                    <!-- -->
                    <br />
                    <asp:Button runat="server" ID="btnSearch" Text="Search" CssClass="button" />
                    <asp:Button runat="server" ID="btnClearSearch" Text="Clear" OnClick="btnClearSearch_Click" CssClass="button" />
                </div>
            </ContentTemplate>
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnSearch" />
            </Triggers>
        </asp:UpdatePanel>

    </form>
</asp:Content>

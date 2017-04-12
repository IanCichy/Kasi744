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
                         CssClass="gridView" >
                        <RowStyle CssClass ="row-gv" />
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
                        FilterExpression="FileName LIKE '%{0}%' AND MapSize LIKE '%{2}%' AND Regions LIKE '%{3}%' AND Agents LIKE '%{4}%' AND StepsTaken LIKE '%{5}%' AND StepLimit LIKE '%{6}%' AND Algorithm LIKE '%{7}%'">
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
                            <asp:ControlParameter Name="Algorithm" ControlID="txtAlgorithm" 
                                PropertyName="Text" Type="String" DefaultValue="" ConvertEmptyStringToNull="false" />
                        </FilterParameters>
                    </asp:SqlDataSource>
                    <br />
                    <asp:TextBox runat="server" ID="txtResults" TextMode="MultiLine" />
                </div>
                <div class="right-half">
                    <br />
                    <asp:Label runat="server" ID="lblFileName" Text="File Name:" CssClass="labelSearch" /> 
                    <asp:TextBox runat="server" ID="txtFileName" CssClass="containerSearch" placeholder="..." AutoPostBack="false" />
                    <br /><br />
                    <!-- -->
                    <asp:Label runat="server" ID="lblMapSize" Text="Map Size:" CssClass="labelSearch" />
                    <asp:TextBox runat="server" ID="txtMapSize" CssClass="containerSearch" placeholder="..." AutoPostBack="false" />
                    <br /><br />
                    <!-- -->
                    <asp:Label runat="server" ID="lblRegions" Text="Regions:" CssClass="labelSearch" />
                    <asp:TextBox runat="server" ID="txtRegions" CssClass="containerSearch" placeholder="..." AutoPostBack="false" />
                    <br /><br />
                    <!-- -->
                    <asp:Label runat="server" ID="lblAgents" Text="Agents:" CssClass="labelSearch" />
                    <asp:TextBox runat="server" ID="txtAgents" CssClass="containerSearch" placeholder="..." AutoPostBack="false" />
                    <br /><br />
                    <!-- -->
                    <asp:Label runat="server" ID="lblSteps" Text="Steps:" CssClass="labelSearch" />
                    <asp:TextBox runat="server" ID="txtSteps" CssClass="containerSearch" placeholder="..." AutoPostBack="false" />
                    <br /><br />
                    <!-- -->
                    <asp:Label runat="server" ID="lblLimit" Text="Limit:" CssClass="labelSearch" />
                    <asp:TextBox runat="server" ID="txtLimit" CssClass="containerSearch" placeholder="..." AutoPostBack="false" />
                    <br /><br />
                    <!-- -->
                    <asp:Label runat="server" ID="lblAlgorithm" Text="Algorithm:" CssClass="labelSearch" />
                    <asp:TextBox runat="server" ID="txtAlgorithm" CssClass="containerSearch" placeholder="..." AutoPostBack="false" />
                    <br /><br />
                    <!-- -->
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

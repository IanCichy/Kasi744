<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Cardinals.aspx.cs" Inherits="Cardinals" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>

<asp:Content ID="Content" ContentPlaceHolderID="body" runat="Server">
    <form runat="server" class="form">
        <asp:ScriptManager EnablePartialRendering="true" EnablePageMethods="true"
            ID="ScriptManager1" runat="server">
        </asp:ScriptManager>

        <script type="text/javascript" id="WindowSize">
            function getPageSize() {
                document.getElementById('<%=PageWidth.ClientID %>').value = document.all ? document.body.clientWidth : window.innerWidth;
                document.getElementById('<%=PageHeight.ClientID %>').value = document.all ? document.body.clientHeight : window.innerHeight;
                __doPostBack('', '');
            }
        </script>
        <asp:HiddenField runat="server" ID="PageWidth" />
        <asp:HiddenField runat="server" ID="PageHeight" />
        <asp:Button runat="server" ID="doNothingButton" OnClientClick="return false;" CssClass="hidden" />

        <br />
        <br />
        <div>
            <asp:UpdatePanel runat="server" ID="UDPContained" UpdateMode="always">
                <ContentTemplate>
                    <div runat="server" id="graphSide" class="left-half">
                        <!-- THIS IS THE MAP -->
                        <asp:UpdatePanel runat="server" ID="UDPViews" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Panel runat="server" ID="viewContainer" CssClass="viewContainer" />
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="btnOneStep" />
                                <asp:AsyncPostBackTrigger ControlID="btnNStep" />
                                <asp:AsyncPostBackTrigger ControlID="btnViewBlock" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>

                    <div runat="server" id="buttonSide" class="right-half">
                        <!-- THIS IS THE CONTROLS -->
                        <asp:Label ID="lblAlgorithmSelected" runat="server" />
                        <br />
                        <br />
                        <asp:Button runat="server" ID="btnViewBlock" OnClick="btnViewBlock_Click" Text="Back to Block View" />
                        <br />
                        <br />
                        <asp:Button runat="server" ID="btnOneStep" OnClick="btnOneStep_Click" Text="Run One Step" />
                        <br />
                        <br />
                        <asp:TextBox runat="server" ID="txtStepCount" placeholder="# Steps" CssClass="txtStepsNumber" />
                        <asp:Button runat="server" ID="btnNStep" OnClick="btnNStep_Click" Text="Run N Steps" />
                        <br />
                        <br />
                        <asp:Label runat="server" ID="lblStepCount" Text="Steps:" />
                        <br />
                        <br />
                        <asp:TextBox runat="server" ID="txtFileName" PlaceHolder="Enter File Name" />
                        <asp:Button runat="server" ID="btnSaveFile" Text="Save" OnClick="btnSaveFile_Click" />
                        <br />
                        <asp:Label ID="lblError" runat="server" Text=""></asp:Label>
                        <br />
                        <br />
                        <asp:Button runat="server" ID="btnNewRun" Text="Start New Run" OnClick="btnNewRun_Click" />
                        <br />
                        <br />
                        <asp:Button runat="server" ID="btnViewOldRuns" Text="View Old Runs" OnClick="btnViewOldRuns_Click" CssClass="button-information" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

            <div runat="server" id="takeInput" class="left-half">
                <div class="preInput">
                    <!-- Pre-run Input -->
                    <!-- Algorithm Selection -->
                    <div class="center">
                        <asp:Label runat="server" ID="lblAlgorithmSelect" Text="Select an algorithm:" />
                        <asp:DropDownList runat="server" ID="ddlAlgorithmSelect" AutoPostBack="true">
                            <asp:ListItem Selected="True" Value="0"> ... </asp:ListItem>
                            <asp:ListItem Value="1"> Free Form </asp:ListItem>
                            <asp:ListItem Value="2"> Constrained-3 </asp:ListItem>
                            <asp:ListItem Value="3"> Constrained-4 </asp:ListItem>
                        </asp:DropDownList>
                        <asp:Button runat="server" ID="btnAlgorithmSelect" Text="Confirm" OnClick="btnAlgorithmSelect_Click" />
                    </div>
                    <!-- Max Steps -->
                    <div class="center">
                        <asp:Label runat="server" ID="lblMaxSteps" Text="Specify a maximum number of steps, or leave blank:" />
                        <asp:TextBox runat="server" ID="txtMaxSteps" Placeholder="Max Steps" AutoPostBack="false" />
                        <asp:Button runat="server" ID="btnMaxSteps" Text="Confirm" OnClick="btnMaxSteps_Click" />
                    </div>
                    <!-- Map Selection -->
                    <div class="center">
                        <asp:Label runat="server" ID="lblMapSelect" Text="Select a map file:" />
                        <asp:FileUpload ID="file" runat="server" />
                        <asp:Button runat="server" ID="btnUploadMap" OnClick="btnUploadMap_Click" Text="Upload" />
                    </div>
                    <asp:Label runat="server" ID="lblInputError" />
                    <!-- Pre-run Input -->
                </div>
            </div>
        </div>
    </form>
</asp:Content>

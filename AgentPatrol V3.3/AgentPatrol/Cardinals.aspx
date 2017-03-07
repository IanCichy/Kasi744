﻿<%@ Page Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Cardinals.aspx.cs" Inherits="Cardinals" EnableEventValidation="false" MaintainScrollPositionOnPostback="true" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>

<asp:Content ID="Content" ContentPlaceHolderID="body" runat="Server">
    <form runat="server">
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
        <br />

        <div class="container">

            <asp:UpdatePanel runat="server" ID="UDPContained" UpdateMode="always">

                <ContentTemplate>

                    <div class="left-half">
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

                    <div class="right-half">
                        <!-- THIS IS THE CONTROLS -->
                        <asp:Button runat="server" ID="btnViewBlock" OnClick="btnViewBlock_Click" Text="Back to Block View" />
                        <br />
                        <asp:Button runat="server" ID="btnOneStep" OnClick="btnOneStep_Click" Text="Run One Step" />
                        <br />
                        <br />
                        <asp:TextBox runat="server" ID="txtStepCount" placeholder="Step Amount" CssClass="txtEnterText" />
                        <br />
                        <asp:Button runat="server" ID="btnNStep" OnClick="btnNStep_Click" Text="Run N Steps" />
                        <br />
                        <br />
                        <asp:Label runat="server" ID="lblStepCount" Text="Steps:" />
                        <br />
                        <br />
                        <asp:TextBox runat="server" ID="txtFileName" PlaceHolder="Enter File Name" />
                        <br />
                        <asp:Button runat="server" ID="btnSaveFile" Text="Save" OnClick="btnSaveFile_Click" />
                        <br />
                        <asp:Label ID="lblError" runat="server" Text=""></asp:Label>
                        <br />
                    </div>

                    <!--THIS IS THE POPUP FOR THE NODE CLICK-->
                    <asp:Button ID="btnFake" runat="server" Style="display: none;" />
                    <ajax:ModalPopupExtender
                        ID="mpe"
                        runat="server"
                        PopupControlID="pnlPopup"
                        TargetControlID="btnFake"
                        OkControlID="btnClose"
                        BackgroundCssClass="modalBackground"
                        PopupDragHandleControlID="pnlPopup" />
                    <asp:Panel
                        ID="pnlPopup"
                        runat="server"
                        CssClass="modalPopup">
                        <div class="popupHeader">
                            Agent Information
                        </div>
                        <div class="body">
                            <b>INFO:</b>
                            <asp:Label ID="lblInfo" runat="server" Text="PLACEHOLDER" /><br />
                        </div>
                        <div class="footer">
                            <asp:Button ID="btnClose" runat="server" Text="Okay" />
                        </div>
                    </asp:Panel>
                    <!--THIS IS THE POPUP FOR THE NODE CLICK-->

                </ContentTemplate>

            </asp:UpdatePanel>

            <div class="right-half-bottom">
                <br />
                <br />
                <asp:Button runat="server" ID="btnNewRun" Text="Start New Run" OnClick="btnNewRun_Click" />
                <asp:FileUpload ID="file" runat="server" />
                <br />
                <asp:Button runat="server" ID="uploadMap" OnClick="uploadMap_Click" Text="Upload Map" />
                <br />
                <br />
                <br />
                <br />
                <br />
                <asp:Button runat="server" ID="btnViewOldRuns" Text="View Old Runs" OnClick="btnViewOldRuns_Click" CssClass="button-OldRuns" />
                <br />
                <br />
            </div>
        </div>
    </form>
</asp:Content>

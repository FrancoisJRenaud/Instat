﻿' Instat-R
' Copyright (C) 2015
'
' This program is free software: you can redistribute it and/or modify
' it under the terms of the GNU General Public License as published by
' the Free Software Foundation, either version 3 of the License, or
' (at your option) any later version.
'
' This program is distributed in the hope that it will be useful,
' but WITHOUT ANY WARRANTY; without even the implied warranty of
' MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
' GNU General Public License for more details.
'
' You should have received a copy of the GNU General Public License k
' along with this program.  If not, see <http://www.gnu.org/licenses/>.

Public Class ucrFilter
    Public bFirstLoad As Boolean
    Private clsFilterFunction As ROperator

    Public Sub New()
        ' This call is required by the designer.
        InitializeComponent()

        ' Add any initialization after the InitializeComponent() call.
        bFirstLoad = True
        clsFilterFunction = New ROperator
    End Sub

    Private Sub ucrFilter_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If bFirstLoad Then
            InitialiseDialog()
            SetDefaults()
            bFirstLoad = False
        End If
    End Sub

    Private Sub InitialiseDialog()
        ucrFilterPreview.Enabled = False
        ucrFilterByReceiver.Selector = ucrSelectorForFitler
        ucrFilterOperation.AddItems({"=", "<", "<=", ">", ">=", "!="})
        ucrFactorLevels.SetAsMultipleSelector()
        ucrFactorLevels.SetReceiver(ucrFilterByReceiver)
        lstFilters.Columns.Add("Variable")
        lstFilters.Columns.Add("Condition")
    End Sub

    Private Sub SetDefaults()
        ucrFilterOperation.SetName("=")
        ucrFilterByReceiver.SetMeAsReceiver()
        VariableTypeProperties()
    End Sub

    Private Sub ucrFilterVariable_SelectionChanged(sender As Object, e As EventArgs) Handles ucrFilterByReceiver.SelectionChanged
        VariableTypeProperties()
    End Sub

    Private Sub VariableTypeProperties()
        Dim bIsFactor As Boolean
        If ucrFilterByReceiver.IsEmpty() Then
            ucrValueForFilter.Visible = False
            lblSelectLevels.Visible = False
            ucrFactorLevels.Visible = False
            cmdToggleSelectAll.Visible = False
            ucrFilterOperation.Visible = False
        Else
            bIsFactor = ucrFilterByReceiver.strCurrDataType = "factor"
            lblSelectLevels.Visible = bIsFactor
            ucrFactorLevels.Visible = bIsFactor
            cmdToggleSelectAll.Visible = bIsFactor
            ucrValueForFilter.Visible = Not bIsFactor
            ucrFilterOperation.Visible = Not bIsFactor
        End If
        SetToggleButtonSettings()
        CheckAddEnabled()
    End Sub

    Private Sub CheckAddEnabled()
        If Not ucrFilterByReceiver.IsEmpty() Then
            If ucrFilterByReceiver.strCurrDataType = "factor" AndAlso ucrFactorLevels.GetSelectedLevels() <> "" Then
                cmdAddFilter.Enabled = True
            ElseIf (Not ucrFilterOperation.IsEmpty) AndAlso (Not ucrValueForFilter.IsEmpty) Then
                cmdAddFilter.Enabled = True
            Else
                cmdAddFilter.Enabled = False
            End If
        Else
            cmdAddFilter.Enabled = False
        End If
    End Sub

    Private Sub cmdAddFilter_Click(sender As Object, e As EventArgs) Handles cmdAddFilter.Click
        Dim clsCurrentCondition As New ROperator
        Dim lviCondition As ListViewItem

        clsCurrentCondition.SetParameter(True, ucrFilterByReceiver.GetVariableNames())
        If ucrFilterByReceiver.strCurrDataType = "factor" Then
            clsCurrentCondition.SetOperation("%in%")
            clsCurrentCondition.SetParameter(False, ucrFactorLevels.GetSelectedLevels())
        Else
            clsCurrentCondition.SetOperation(ucrFilterOperation.GetText())
            clsCurrentCondition.SetParameter(False, ucrValueForFilter.GetText())
        End If
        lviCondition = New ListViewItem({ucrFilterByReceiver.GetVariableNames(), clsCurrentCondition.strOperation & " " & clsCurrentCondition.clsRightParameter.strArgumentValue})
        lstFilters.Items.Add(lviCondition)
        ucrFilterByReceiver.Clear()
        CheckAddEnabled()
    End Sub

    Private Sub cmdToggleSelectAll_Click(sender As Object, e As EventArgs) Handles cmdToggleSelectAll.Click
        SetToggleButtonSettings()
    End Sub

    Private Sub SetToggleButtonSettings()
        If cmdToggleSelectAll.FlatStyle = FlatStyle.Popup Then
            ucrFactorLevels.SetSelectionAllLevels(True)
            cmdToggleSelectAll.Text = "Deselect All"
            cmdToggleSelectAll.FlatStyle = FlatStyle.Flat
        Else
            ucrFactorLevels.SetSelectionAllLevels(False)
            cmdToggleSelectAll.Text = "Select All"
            cmdToggleSelectAll.FlatStyle = FlatStyle.Popup
        End If
        CheckAddEnabled()
    End Sub

    Private Sub ucrFilterOperation_NameChanged() Handles ucrFilterOperation.NameChanged
        CheckAddEnabled()
    End Sub

    Private Sub ucrValueForFilter_NameChanged() Handles ucrValueForFilter.NameChanged
        CheckAddEnabled()
    End Sub

    Private Sub ucrFactorLevels_SelectedLevelChanged() Handles ucrFactorLevels.SelectedLevelChanged
        SetToggleButtonSettings()
        CheckAddEnabled()
    End Sub
End Class
<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.lblStatus = New System.Windows.Forms.Label()
        Me.Btn_ImportarXml = New System.Windows.Forms.Button()
        Me.grdNota = New System.Windows.Forms.DataGridView()
        Me.Label1 = New System.Windows.Forms.Label()
        CType(Me.grdNota,System.ComponentModel.ISupportInitialize).BeginInit
        Me.SuspendLayout
        '
        'lblStatus
        '
        Me.lblStatus.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.lblStatus.ForeColor = System.Drawing.Color.SteelBlue
        Me.lblStatus.Location = New System.Drawing.Point(37, 93)
        Me.lblStatus.Name = "lblStatus"
        Me.lblStatus.Size = New System.Drawing.Size(797, 35)
        Me.lblStatus.TabIndex = 242
        Me.lblStatus.Text = "Situação do Processamento: Não executado"
        Me.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        '
        'Btn_ImportarXml
        '
        Me.Btn_ImportarXml.BackColor = System.Drawing.SystemColors.ControlLightLight
        Me.Btn_ImportarXml.FlatStyle = System.Windows.Forms.FlatStyle.Flat
        Me.Btn_ImportarXml.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0,Byte))
        Me.Btn_ImportarXml.ForeColor = System.Drawing.Color.SteelBlue
        Me.Btn_ImportarXml.Location = New System.Drawing.Point(329, 12)
        Me.Btn_ImportarXml.Name = "Btn_ImportarXml"
        Me.Btn_ImportarXml.Size = New System.Drawing.Size(187, 67)
        Me.Btn_ImportarXml.TabIndex = 241
        Me.Btn_ImportarXml.Text = "Importar XML"
        Me.Btn_ImportarXml.UseVisualStyleBackColor = false
        '
        'grdNota
        '
        Me.grdNota.AllowUserToAddRows = false
        Me.grdNota.AllowUserToDeleteRows = false
        Me.grdNota.AllowUserToOrderColumns = true
        Me.grdNota.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.grdNota.Location = New System.Drawing.Point(12, 144)
        Me.grdNota.Name = "grdNota"
        Me.grdNota.Size = New System.Drawing.Size(845, 313)
        Me.grdNota.TabIndex = 243
        '
        'Label1
        '
        Me.Label1.AutoSize = true
        Me.Label1.Location = New System.Drawing.Point(12, 462)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(39, 13)
        Me.Label1.TabIndex = 244
        Me.Label1.Text = "Label1"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6!, 13!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(880, 482)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.grdNota)
        Me.Controls.Add(Me.lblStatus)
        Me.Controls.Add(Me.Btn_ImportarXml)
        Me.Name = "Form1"
        Me.Text = "Form1"
        CType(Me.grdNota,System.ComponentModel.ISupportInitialize).EndInit
        Me.ResumeLayout(false)
        Me.PerformLayout

End Sub

    Friend WithEvents lblStatus As Label
    Friend WithEvents Btn_ImportarXml As Button
    Friend WithEvents grdNota As DataGridView
    Friend WithEvents Label1 As Label
End Class

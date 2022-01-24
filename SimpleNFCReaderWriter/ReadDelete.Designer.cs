namespace SimpleNFCReaderWriter
{
    partial class ReadDelete
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btn_readCard = new System.Windows.Forms.Button();
            this.btn_Delete = new System.Windows.Forms.Button();
            this.cb_Connection = new System.Windows.Forms.CheckBox();
            this.tb_Data = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_Init = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_readCard
            // 
            this.btn_readCard.Location = new System.Drawing.Point(165, 32);
            this.btn_readCard.Name = "btn_readCard";
            this.btn_readCard.Size = new System.Drawing.Size(104, 23);
            this.btn_readCard.TabIndex = 9;
            this.btn_readCard.Text = "Read card";
            this.btn_readCard.UseVisualStyleBackColor = true;
            this.btn_readCard.Click += new System.EventHandler(this.btn_readCard_Click);
            // 
            // btn_Delete
            // 
            this.btn_Delete.Location = new System.Drawing.Point(48, 32);
            this.btn_Delete.Name = "btn_Delete";
            this.btn_Delete.Size = new System.Drawing.Size(104, 23);
            this.btn_Delete.TabIndex = 8;
            this.btn_Delete.Text = "Delete";
            this.btn_Delete.UseVisualStyleBackColor = true;
            this.btn_Delete.Click += new System.EventHandler(this.btn_Delete_Click);
            // 
            // cb_Connection
            // 
            this.cb_Connection.AutoSize = true;
            this.cb_Connection.Location = new System.Drawing.Point(275, 8);
            this.cb_Connection.Name = "cb_Connection";
            this.cb_Connection.Size = new System.Drawing.Size(80, 17);
            this.cb_Connection.TabIndex = 7;
            this.cb_Connection.Text = "Connection";
            this.cb_Connection.UseVisualStyleBackColor = true;
            this.cb_Connection.CheckedChanged += new System.EventHandler(this.cb_Connection_CheckedChanged);
            // 
            // tb_Data
            // 
            this.tb_Data.Location = new System.Drawing.Point(48, 6);
            this.tb_Data.Name = "tb_Data";
            this.tb_Data.Size = new System.Drawing.Size(221, 20);
            this.tb_Data.TabIndex = 6;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(30, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Data";
            // 
            // btn_Init
            // 
            this.btn_Init.Location = new System.Drawing.Point(48, 61);
            this.btn_Init.Name = "btn_Init";
            this.btn_Init.Size = new System.Drawing.Size(221, 23);
            this.btn_Init.TabIndex = 10;
            this.btn_Init.Text = "Initialisation";
            this.btn_Init.UseVisualStyleBackColor = true;
            this.btn_Init.Click += new System.EventHandler(this.btn_Init_Click);
            // 
            // ReadDelete
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(361, 89);
            this.Controls.Add(this.btn_Init);
            this.Controls.Add(this.btn_readCard);
            this.Controls.Add(this.btn_Delete);
            this.Controls.Add(this.cb_Connection);
            this.Controls.Add(this.tb_Data);
            this.Controls.Add(this.label1);
            this.Name = "ReadDelete";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "ReadDelete";
            this.Load += new System.EventHandler(this.ReadDelete_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btn_readCard;
        private System.Windows.Forms.Button btn_Delete;
        private System.Windows.Forms.CheckBox cb_Connection;
        private System.Windows.Forms.TextBox tb_Data;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_Init;
    }
}
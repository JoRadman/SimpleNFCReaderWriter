namespace SimpleNFCReaderWriter
{
    partial class StartScreen
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
            this.btn_Write = new System.Windows.Forms.Button();
            this.btn_ReadDelete = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btn_Write
            // 
            this.btn_Write.Location = new System.Drawing.Point(12, 12);
            this.btn_Write.Name = "btn_Write";
            this.btn_Write.Size = new System.Drawing.Size(107, 23);
            this.btn_Write.TabIndex = 0;
            this.btn_Write.Text = "WRITE";
            this.btn_Write.UseVisualStyleBackColor = true;
            this.btn_Write.Click += new System.EventHandler(this.btn_Write_Click);
            // 
            // btn_ReadDelete
            // 
            this.btn_ReadDelete.Location = new System.Drawing.Point(139, 12);
            this.btn_ReadDelete.Name = "btn_ReadDelete";
            this.btn_ReadDelete.Size = new System.Drawing.Size(102, 23);
            this.btn_ReadDelete.TabIndex = 1;
            this.btn_ReadDelete.Text = "READ/DELETE";
            this.btn_ReadDelete.UseVisualStyleBackColor = true;
            this.btn_ReadDelete.Click += new System.EventHandler(this.btn_ReadDelete_Click);
            // 
            // StartScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(253, 43);
            this.Controls.Add(this.btn_ReadDelete);
            this.Controls.Add(this.btn_Write);
            this.Name = "StartScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "StartScreen";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btn_Write;
        private System.Windows.Forms.Button btn_ReadDelete;
    }
}
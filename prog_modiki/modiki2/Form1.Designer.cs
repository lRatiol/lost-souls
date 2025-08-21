namespace modiki2
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btn1 = new Button();
            logs = new TextBox();
            progressBar = new ProgressBar();
            btnObserver = new Button();
            textBoxPath = new TextBox();
            SuspendLayout();
            // 
            // btn1
            // 
            btn1.Location = new Point(280, 41);
            btn1.Name = "btn1";
            btn1.Size = new Size(75, 23);
            btn1.TabIndex = 0;
            btn1.Text = "обновить";
            btn1.UseVisualStyleBackColor = true;
            btn1.Click += btn1_Click;
            // 
            // logs
            // 
            logs.Location = new Point(0, 70);
            logs.Multiline = true;
            logs.Name = "logs";
            logs.ScrollBars = ScrollBars.Vertical;
            logs.Size = new Size(648, 290);
            logs.TabIndex = 1;
            // 
            // progressBar
            // 
            progressBar.Location = new Point(12, 41);
            progressBar.Name = "progressBar";
            progressBar.Size = new Size(262, 23);
            progressBar.TabIndex = 2;
            // 
            // btnObserver
            // 
            btnObserver.Location = new Point(560, 12);
            btnObserver.Name = "btnObserver";
            btnObserver.Size = new Size(75, 23);
            btnObserver.TabIndex = 0;
            btnObserver.Text = "Обзор";
            btnObserver.UseVisualStyleBackColor = true;
            btnObserver.Click += btnObserver_Click;
            // 
            // textBoxPath
            // 
            textBoxPath.Location = new Point(12, 12);
            textBoxPath.Name = "textBoxPath";
            textBoxPath.ScrollBars = ScrollBars.Vertical;
            textBoxPath.Size = new Size(542, 23);
            textBoxPath.TabIndex = 3;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(647, 360);
            Controls.Add(textBoxPath);
            Controls.Add(progressBar);
            Controls.Add(logs);
            Controls.Add(btnObserver);
            Controls.Add(btn1);
            Name = "Form1";
            Text = "Form1";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btn1;
        private TextBox logs;
        private ProgressBar progressBar;
        private Button btnObserver;
        private TextBox textBoxPath;
    }
}

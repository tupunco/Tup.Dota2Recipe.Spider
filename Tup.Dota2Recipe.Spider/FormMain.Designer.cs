namespace Tup.Dota2Recipe.Spider
{
    partial class MainForm
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.ButtonGetHeroData = new System.Windows.Forms.Button();
            this.ButtonGetItemsData = new System.Windows.Forms.Button();
            this.ListBoxMsg = new System.Windows.Forms.ListBox();
            this.CheckBoxHeroDetail = new System.Windows.Forms.CheckBox();
            this.CheckBoxHeroImage = new System.Windows.Forms.CheckBox();
            this.CheckBoxHeroAbilityImage = new System.Windows.Forms.CheckBox();
            this.CheckBoxItemsImage = new System.Windows.Forms.CheckBox();
            this.GroupBoxHero = new System.Windows.Forms.GroupBox();
            this.LabelDota2Itembuilds = new System.Windows.Forms.Label();
            this.ButtonBrowserDota2Itembuilds = new System.Windows.Forms.Button();
            this.TextBoxDota2Itembuilds = new System.Windows.Forms.TextBox();
            this.GroupBoxHero.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonGetHeroData
            // 
            this.ButtonGetHeroData.Location = new System.Drawing.Point(12, 57);
            this.ButtonGetHeroData.Name = "ButtonGetHeroData";
            this.ButtonGetHeroData.Size = new System.Drawing.Size(120, 23);
            this.ButtonGetHeroData.TabIndex = 0;
            this.ButtonGetHeroData.Text = "Get Hero Data";
            this.ButtonGetHeroData.UseVisualStyleBackColor = true;
            this.ButtonGetHeroData.Click += new System.EventHandler(this.ButtonGetHeroData_Click);
            // 
            // ButtonGetItemsData
            // 
            this.ButtonGetItemsData.Location = new System.Drawing.Point(13, 12);
            this.ButtonGetItemsData.Name = "ButtonGetItemsData";
            this.ButtonGetItemsData.Size = new System.Drawing.Size(120, 23);
            this.ButtonGetItemsData.TabIndex = 1;
            this.ButtonGetItemsData.Text = "Get Items Data";
            this.ButtonGetItemsData.UseVisualStyleBackColor = true;
            this.ButtonGetItemsData.Click += new System.EventHandler(this.ButtonGetItemsData_Click);
            // 
            // ListBoxMsg
            // 
            this.ListBoxMsg.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ListBoxMsg.FormattingEnabled = true;
            this.ListBoxMsg.ItemHeight = 12;
            this.ListBoxMsg.Location = new System.Drawing.Point(13, 182);
            this.ListBoxMsg.Name = "ListBoxMsg";
            this.ListBoxMsg.Size = new System.Drawing.Size(592, 304);
            this.ListBoxMsg.TabIndex = 2;
            // 
            // CheckBoxHeroDetail
            // 
            this.CheckBoxHeroDetail.AutoSize = true;
            this.CheckBoxHeroDetail.Checked = true;
            this.CheckBoxHeroDetail.CheckState = System.Windows.Forms.CheckState.Checked;
            this.CheckBoxHeroDetail.Location = new System.Drawing.Point(6, 20);
            this.CheckBoxHeroDetail.Name = "CheckBoxHeroDetail";
            this.CheckBoxHeroDetail.Size = new System.Drawing.Size(108, 16);
            this.CheckBoxHeroDetail.TabIndex = 3;
            this.CheckBoxHeroDetail.Text = "detail/ability";
            this.CheckBoxHeroDetail.UseVisualStyleBackColor = true;
            // 
            // CheckBoxHeroImage
            // 
            this.CheckBoxHeroImage.AutoSize = true;
            this.CheckBoxHeroImage.Location = new System.Drawing.Point(6, 43);
            this.CheckBoxHeroImage.Name = "CheckBoxHeroImage";
            this.CheckBoxHeroImage.Size = new System.Drawing.Size(84, 16);
            this.CheckBoxHeroImage.TabIndex = 4;
            this.CheckBoxHeroImage.Text = "hero image";
            this.CheckBoxHeroImage.UseVisualStyleBackColor = true;
            // 
            // CheckBoxHeroAbilityImage
            // 
            this.CheckBoxHeroAbilityImage.AutoSize = true;
            this.CheckBoxHeroAbilityImage.Location = new System.Drawing.Point(6, 65);
            this.CheckBoxHeroAbilityImage.Name = "CheckBoxHeroAbilityImage";
            this.CheckBoxHeroAbilityImage.Size = new System.Drawing.Size(102, 16);
            this.CheckBoxHeroAbilityImage.TabIndex = 5;
            this.CheckBoxHeroAbilityImage.Text = "ability image";
            this.CheckBoxHeroAbilityImage.UseVisualStyleBackColor = true;
            // 
            // CheckBoxItemsImage
            // 
            this.CheckBoxItemsImage.AutoSize = true;
            this.CheckBoxItemsImage.Location = new System.Drawing.Point(172, 16);
            this.CheckBoxItemsImage.Name = "CheckBoxItemsImage";
            this.CheckBoxItemsImage.Size = new System.Drawing.Size(90, 16);
            this.CheckBoxItemsImage.TabIndex = 6;
            this.CheckBoxItemsImage.Text = "items image";
            this.CheckBoxItemsImage.UseVisualStyleBackColor = true;
            // 
            // GroupBoxHero
            // 
            this.GroupBoxHero.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupBoxHero.Controls.Add(this.LabelDota2Itembuilds);
            this.GroupBoxHero.Controls.Add(this.ButtonBrowserDota2Itembuilds);
            this.GroupBoxHero.Controls.Add(this.TextBoxDota2Itembuilds);
            this.GroupBoxHero.Controls.Add(this.CheckBoxHeroDetail);
            this.GroupBoxHero.Controls.Add(this.CheckBoxHeroImage);
            this.GroupBoxHero.Controls.Add(this.CheckBoxHeroAbilityImage);
            this.GroupBoxHero.Location = new System.Drawing.Point(166, 44);
            this.GroupBoxHero.Name = "GroupBoxHero";
            this.GroupBoxHero.Size = new System.Drawing.Size(439, 125);
            this.GroupBoxHero.TabIndex = 7;
            this.GroupBoxHero.TabStop = false;
            // 
            // LabelDota2Itembuilds
            // 
            this.LabelDota2Itembuilds.AutoSize = true;
            this.LabelDota2Itembuilds.Location = new System.Drawing.Point(6, 92);
            this.LabelDota2Itembuilds.Name = "LabelDota2Itembuilds";
            this.LabelDota2Itembuilds.Size = new System.Drawing.Size(95, 12);
            this.LabelDota2Itembuilds.TabIndex = 8;
            this.LabelDota2Itembuilds.Text = "Dota2Itembuilds";
            // 
            // ButtonBrowserDota2Itembuilds
            // 
            this.ButtonBrowserDota2Itembuilds.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonBrowserDota2Itembuilds.Location = new System.Drawing.Point(398, 87);
            this.ButtonBrowserDota2Itembuilds.Name = "ButtonBrowserDota2Itembuilds";
            this.ButtonBrowserDota2Itembuilds.Size = new System.Drawing.Size(35, 23);
            this.ButtonBrowserDota2Itembuilds.TabIndex = 7;
            this.ButtonBrowserDota2Itembuilds.Text = "...";
            this.ButtonBrowserDota2Itembuilds.UseVisualStyleBackColor = true;
            this.ButtonBrowserDota2Itembuilds.Click += new System.EventHandler(this.ButtonBrowerDota2Itembuilds_Click);
            // 
            // TextBoxDota2Itembuilds
            // 
            this.TextBoxDota2Itembuilds.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxDota2Itembuilds.Location = new System.Drawing.Point(107, 87);
            this.TextBoxDota2Itembuilds.Name = "TextBoxDota2Itembuilds";
            this.TextBoxDota2Itembuilds.Size = new System.Drawing.Size(285, 21);
            this.TextBoxDota2Itembuilds.TabIndex = 6;
            this.TextBoxDota2Itembuilds.Text = "F:\\Steam_Dota2\\SteamApps\\common\\dota 2 beta\\dota\\itembuilds";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(617, 493);
            this.Controls.Add(this.GroupBoxHero);
            this.Controls.Add(this.CheckBoxItemsImage);
            this.Controls.Add(this.ListBoxMsg);
            this.Controls.Add(this.ButtonGetItemsData);
            this.Controls.Add(this.ButtonGetHeroData);
            this.Name = "MainForm";
            this.Text = "Tup.Dota2Recipe.Spider";
            this.GroupBoxHero.ResumeLayout(false);
            this.GroupBoxHero.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ButtonGetHeroData;
        private System.Windows.Forms.Button ButtonGetItemsData;
        private System.Windows.Forms.ListBox ListBoxMsg;
        private System.Windows.Forms.CheckBox CheckBoxHeroDetail;
        private System.Windows.Forms.CheckBox CheckBoxHeroImage;
        private System.Windows.Forms.CheckBox CheckBoxHeroAbilityImage;
        private System.Windows.Forms.CheckBox CheckBoxItemsImage;
        private System.Windows.Forms.GroupBox GroupBoxHero;
        private System.Windows.Forms.Label LabelDota2Itembuilds;
        private System.Windows.Forms.Button ButtonBrowserDota2Itembuilds;
        private System.Windows.Forms.TextBox TextBoxDota2Itembuilds;
    }
}


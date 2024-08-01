namespace WindowVoice
{
    partial class Form1_main
    {
        /// <summary>
        /// 필수 디자이너 변수입니다.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 사용 중인 모든 리소스를 정리합니다.
        /// </summary>
        /// <param name="disposing">관리되는 리소스를 삭제해야 하면 true이고, 그렇지 않으면 false입니다.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form 디자이너에서 생성한 코드

        /// <summary>
        /// 디자이너 지원에 필요한 메서드입니다. 
        /// 이 메서드의 내용을 코드 편집기로 수정하지 마세요.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox1_speech = new System.Windows.Forms.TextBox();
            this.checkBox1_enter_to_clear = new System.Windows.Forms.CheckBox();
            this.button1_enter = new System.Windows.Forms.Button();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox2_Output = new System.Windows.Forms.ComboBox();
            this.comboBox_Input = new System.Windows.Forms.ComboBox();
            this.waveViewer_Input = new NAudio.Gui.WaveViewer();
            this.waveViewer_Text = new NAudio.Gui.WaveViewer();
            this.SuspendLayout();
            // 
            // textBox1_speech
            // 
            this.textBox1_speech.Location = new System.Drawing.Point(13, 13);
            this.textBox1_speech.Multiline = true;
            this.textBox1_speech.Name = "textBox1_speech";
            this.textBox1_speech.Size = new System.Drawing.Size(461, 241);
            this.textBox1_speech.TabIndex = 0;
            this.textBox1_speech.Text = "문장 입력";
            // 
            // checkBox1_enter_to_clear
            // 
            this.checkBox1_enter_to_clear.AutoSize = true;
            this.checkBox1_enter_to_clear.Location = new System.Drawing.Point(13, 413);
            this.checkBox1_enter_to_clear.Name = "checkBox1_enter_to_clear";
            this.checkBox1_enter_to_clear.Size = new System.Drawing.Size(148, 16);
            this.checkBox1_enter_to_clear.TabIndex = 1;
            this.checkBox1_enter_to_clear.Text = "엔터누르면 문장지우기";
            this.checkBox1_enter_to_clear.UseVisualStyleBackColor = true;
            // 
            // button1_enter
            // 
            this.button1_enter.Location = new System.Drawing.Point(12, 260);
            this.button1_enter.Name = "button1_enter";
            this.button1_enter.Size = new System.Drawing.Size(106, 55);
            this.button1_enter.TabIndex = 2;
            this.button1_enter.Text = "재생(Enter)";
            this.button1_enter.UseVisualStyleBackColor = true;
            this.button1_enter.Click += new System.EventHandler(this.button1_enter_Click);
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(491, 38);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(121, 20);
            this.comboBox1.TabIndex = 3;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(489, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "Language";
            // 
            // comboBox2_Output
            // 
            this.comboBox2_Output.FormattingEnabled = true;
            this.comboBox2_Output.Location = new System.Drawing.Point(491, 409);
            this.comboBox2_Output.Name = "comboBox2_Output";
            this.comboBox2_Output.Size = new System.Drawing.Size(121, 20);
            this.comboBox2_Output.TabIndex = 5;
            this.comboBox2_Output.SelectedIndexChanged += new System.EventHandler(this.comboBox2_Output_SelectedIndexChanged);
            // 
            // comboBox_Input
            // 
            this.comboBox_Input.FormattingEnabled = true;
            this.comboBox_Input.Location = new System.Drawing.Point(491, 383);
            this.comboBox_Input.Name = "comboBox_Input";
            this.comboBox_Input.Size = new System.Drawing.Size(121, 20);
            this.comboBox_Input.TabIndex = 6;
            this.comboBox_Input.SelectedIndexChanged += new System.EventHandler(this.comboBox_Input_SelectedIndexChanged);
            // 
            // waveViewer_Input
            // 
            this.waveViewer_Input.Location = new System.Drawing.Point(352, 383);
            this.waveViewer_Input.Name = "waveViewer_Input";
            this.waveViewer_Input.SamplesPerPixel = 128;
            this.waveViewer_Input.Size = new System.Drawing.Size(133, 20);
            this.waveViewer_Input.StartPosition = ((long)(0));
            this.waveViewer_Input.TabIndex = 7;
            this.waveViewer_Input.WaveStream = null;
            // 
            // waveViewer_Text
            // 
            this.waveViewer_Text.Location = new System.Drawing.Point(124, 260);
            this.waveViewer_Text.Name = "waveViewer_Text";
            this.waveViewer_Text.SamplesPerPixel = 128;
            this.waveViewer_Text.Size = new System.Drawing.Size(350, 55);
            this.waveViewer_Text.StartPosition = ((long)(0));
            this.waveViewer_Text.TabIndex = 8;
            this.waveViewer_Text.WaveStream = null;
            // 
            // Form1_main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(624, 441);
            this.Controls.Add(this.waveViewer_Text);
            this.Controls.Add(this.waveViewer_Input);
            this.Controls.Add(this.comboBox_Input);
            this.Controls.Add(this.comboBox2_Output);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this.button1_enter);
            this.Controls.Add(this.checkBox1_enter_to_clear);
            this.Controls.Add(this.textBox1_speech);
            this.Name = "Form1_main";
            this.Text = "WindowVoice";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1_speech;
        private System.Windows.Forms.CheckBox checkBox1_enter_to_clear;
        private System.Windows.Forms.Button button1_enter;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboBox2_Output;
        private System.Windows.Forms.ComboBox comboBox_Input;
        private NAudio.Gui.WaveViewer waveViewer_Input;
        private NAudio.Gui.WaveViewer waveViewer_Text;
    }
}


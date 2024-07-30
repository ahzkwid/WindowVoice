using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Windows.Input;
using System.Speech.Synthesis;

namespace WindowVoice
{
    public partial class Form1_main : Form
    {
        private static SpeechSynthesizer synth = new SpeechSynthesizer();
        public Form1_main()
        {
            InitializeComponent();
            
            synth.SetOutputToDefaultAudioDevice();
            //synth.SelectVoice("Microsoft Server Speech Text to Speech Voice (ko-KR, Heami)");
            //synth.SelectVoice("Microsoft Server Speech Text to Speech Voice (ko-KR, TELE)");
            //synth.SelectVoiceByHints(VoiceGender.Male);
            //synth.SelectVoice("Microsoft Zira Desktop");
            //synth.SelectVoice("Microsoft Heami Desktop");

            textBox1_speech.KeyDown += new System.Windows.Forms.KeyEventHandler(this.key_down);
        }
        private void key_down(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)//ESC눌렀을떄
            {
                textBox1_speech.Text = "";
            }
            if ((e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)) //엔터 눌렀을때
            {
                if (((Control.ModifierKeys & Keys.Alt) == Keys.Alt)) //알트눌려있을때
                {
                    int pos = textBox1_speech.SelectionStart;//텍스트박스 포인터 위치
                    textBox1_speech.Text += "\r\n"; //엔터추가 \r을 붙여야 연속추가가 되는듯
                    textBox1_speech.Select(pos + 2, 0); //텍스트박스 포인터 복구하고 엔터 길이만큼 이동
                }
                else
                {
                    e.SuppressKeyPress = true;//엔터무시
                    speech();
                    if (checkBox1_enter_to_clear.Checked == true) //엔터 누를떄 지우기 체크되어있을경우
                    {
                        textBox1_speech.Text = "";
                    }
                }
            }
        }
        private void speech()
        {
            if (textBox1_speech.Text == "문장 입력")
            {
                textBox1_speech.Text = "";
            }
            if (textBox1_speech.Text != "")
            {
                System.Threading.ThreadPool.QueueUserWorkItem(speech, textBox1_speech.Text); //스레드풀
                //speech(textBox1_speech.Text);
            }
        }
        private void speech(Object str)
        {
            synth.Speak((string)str);
        }
        private void speech(string str)
        {
            synth.Speak(str);
        }

        private void button1_enter_Click(object sender, EventArgs e)
        {
            speech();
        }
    }
}

using System;
using System.Linq;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Globalization;
using System.IO;
using NAudio.Wave;


    
using System.Speech.Recognition;

namespace WindowVoice
{
    public partial class Form1_main : Form
    {
        SpeechSynthesizer synth = new SpeechSynthesizer();



        WaveOutEvent outputDevice;
        public Form1_main()
        {
            InitializeComponent();
            LoadRecognize();


            synth.SetOutputToDefaultAudioDevice();
            //synth.SelectVoice("Microsoft Server Speech Text to Speech Voice (ko-KR, Heami)");
            //synth.SelectVoice("Microsoft Server Speech Text to Speech Voice (ko-KR, TELE)");
            //synth.SelectVoiceByHints(VoiceGender.Male);
            //synth.SelectVoice("Microsoft Zira Desktop");
            //synth.SelectVoice("Microsoft Heami Desktop");

            textBox1_speech.KeyDown += new System.Windows.Forms.KeyEventHandler(this.key_down);

            foreach (var voice in synth.GetInstalledVoices())
            {
                var name = voice.VoiceInfo.Name;
                comboBox1.Items.Add(VoiceNameToLanguage(name));
            }
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }

            for (int i = 0; i< NAudio.Wave.WaveOut.DeviceCount; i++)
            {
                var caps = NAudio.Wave.WaveOut.GetCapabilities(i);
                comboBox2_Output.Items.Add(caps.ProductName);
            }
            if (comboBox2_Output.Items.Count > 0)
            { 
                comboBox2_Output.SelectedIndex = 0;
                outputDevice = new WaveOutEvent() { DeviceNumber = 0 };
            }

            var currentCulture = CultureInfo.CurrentUICulture;
            switch (currentCulture.Name)
            {
                case "ko-KR":
                    break;
                case "ja-JP":
                    textBox1_speech.Text = "文章入力";
                    checkBox1_enter_to_clear.Text = "エンターを押すと文章をクリア";
                    break;
                default:
                    button1_enter.Text = "InsertText";
                    checkBox1_enter_to_clear.Text = "Press Enter to delete sentence";
                    break;
            }
            switch (currentCulture.Name)
            {
                case "ko-KR":
                    break;
                default:
                    button1_enter.Text = "Enter";
                    break;
            }








        }
        void LoadRecognize()
        {
            {
                var installedRecognizers = SpeechRecognitionEngine.InstalledRecognizers();
                foreach (var recognizerInfo in installedRecognizers)
                {
                    Console.WriteLine(recognizerInfo.Culture);
                }


                var selectedCulture = installedRecognizers.FirstOrDefault();

                if (selectedCulture != null)
                {

                    var recognizer = new SpeechRecognitionEngine(selectedCulture);

                    recognizer.LoadGrammar(new DictationGrammar());

                    recognizer.SpeechRecognized += (sender, e) =>
                    {
                        textBox1_speech.Text = e.Result.Text;
                        Speech(e.Result.Text);
                    };

                    recognizer.SetInputToDefaultAudioDevice();

                    recognizer.RecognizeAsync(RecognizeMode.Multiple);

                }
            }

        }
        string VoiceNameToLanguage(string name)
        {
            switch (name)
            {
                case "Microsoft Heami Desktop":
                    return "KR";
                case "Microsoft Haruka Desktop":
                    return "JP";
                case "Microsoft Zira Desktop":
                    return "EN";
            }
            return name;
        }
        string LanguageToVoiceName(string name)
        {
            switch (name)
            {
                case "KR":
                    return "Microsoft Heami Desktop";
                case "JP":
                    return "Microsoft Haruka Desktop";
                case "EN":
                    return "Microsoft Zira Desktop";
            }
            return name;
        }
        void key_down(object sender, KeyEventArgs e)
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
                    Speech();
                    if (checkBox1_enter_to_clear.Checked == true) //엔터 누를떄 지우기 체크되어있을경우
                    {
                        textBox1_speech.Text = "";
                    }
                }
            }
        }
        void Speech()
        {
            /*
            if (textBox1_speech.Text == "문장 입력")
            {
                textBox1_speech.Text = "";
            }
            */
            if (textBox1_speech.Text != "")
            {
                System.Threading.ThreadPool.QueueUserWorkItem(Speech, textBox1_speech.Text); //스레드풀
                //speech(textBox1_speech.Text);
            }
        }
        void Speech(Object str)
        {
            Speech((string)str);
        }
        void Speech(string str)
        {
            //synth.Speak(str);




            if (outputDevice != null)
            {
                outputDevice.Stop();
            }

            var waveProvider = new SpeechToWaveProvider(synth, (string)str);
            outputDevice.Init(waveProvider);
            outputDevice.Play();

            /*

            using (var streamAudio = new MemoryStream())
            {

                // Create a SoundPlayer instance to play the output audio file.
                var m_SoundPlayer = new System.Media.SoundPlayer();

                // Configure the synthesizer to output to an audio stream.
                synth.SetOutputToWaveStream(streamAudio);

                // Speak a phrase.
                synth.Speak(str);
                streamAudio.Position = 0;
                m_SoundPlayer.Stream = streamAudio;
                m_SoundPlayer.Play();

                // Set the synthesizer output to null to release the stream.
                synth.SetOutputToNull();

            }
            */







        }

        void button1_enter_Click(object sender, EventArgs e)
        {
            Speech();
        }

        void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //보이스 선택
            synth.SelectVoice(LanguageToVoiceName(comboBox1.Text));
        }

        void comboBox2_Output_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (outputDevice != null)
            {
                outputDevice.Stop();
                outputDevice.Dispose();
                outputDevice = null;
            }

            outputDevice = new WaveOutEvent
            {
                DeviceNumber = comboBox2_Output.SelectedIndex
            };
        }
        public class SpeechToWaveProvider : IWaveProvider
        {
            private readonly BufferedWaveProvider waveProvider;

            public SpeechToWaveProvider(SpeechSynthesizer synth, string text)
            {
                var memoryStream = new MemoryStream();
                synth.SetOutputToWaveStream(memoryStream);
                synth.Speak(text);
                synth.SetOutputToDefaultAudioDevice();

                var waveFormat = new WaveFormat(22000, 1);
                waveProvider = new BufferedWaveProvider(waveFormat)
                {
                    BufferLength = (int)memoryStream.Length
                };

                memoryStream.Position = 0;
                var buffer = new byte[memoryStream.Length];
                memoryStream.Read(buffer, 0, buffer.Length);
                waveProvider.AddSamples(buffer, 0, buffer.Length);
            }

            public WaveFormat WaveFormat => waveProvider.WaveFormat;

            public int Read(byte[] buffer, int offset, int count)
            {
                return waveProvider.Read(buffer, offset, count);
            }
        }
    }
}

using System;
using System.Linq;
using System.Windows.Forms;
using System.Speech.Synthesis;
using System.Globalization;
using System.IO;
using NAudio.Wave;



using System.Speech.Recognition;
using System.Speech.AudioFormat;
using System.Diagnostics;
using NAudio.Utils;
using SpeechLib;
//using Microsoft.Speech.Recognition;

namespace WindowVoice
{
    public partial class Form1_main : Form
    {
        SpeechSynthesizer synth = new SpeechSynthesizer();



        WaveOutEvent outputDevice;
        WaveInEvent inputDevice;

        SpeechRecognitionEngine recognizer;
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

            foreach (var voice in synth.GetInstalledVoices())
            {
                var name = voice.VoiceInfo.Name;
                comboBox1.Items.Add(VoiceNameToLanguage(name));
            }
            if (comboBox1.Items.Count > 0)
            {
                comboBox1.SelectedIndex = 0;
            }

            for (int i = 0; i < NAudio.Wave.WaveOut.DeviceCount; i++)
            {
                var caps = NAudio.Wave.WaveOut.GetCapabilities(i);
                comboBox2_Output.Items.Add(caps.ProductName);
            }
            if (comboBox2_Output.Items.Count > 0)
            {
                outputDevice = new WaveOutEvent() ;
                comboBox2_Output.SelectedIndex = 0;
            }

            for (int i = 0; i < NAudio.Wave.WaveIn.DeviceCount; i++)
            {
                var caps = NAudio.Wave.WaveIn.GetCapabilities(i);
                comboBox_Input.Items.Add(caps.ProductName);
            }

            if (comboBox_Input.Items.Count > 0)
            {

                LoadRecognize();
                inputDevice = new WaveInEvent() ;
                inputDevice.DataAvailable += OnDataAvailable;
                //inputDevice.BufferMilliseconds =100;

                comboBox_Input.SelectedIndex = 0;

                //inputDevice.StartRecording();
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
        private MemoryStream audioStream = new MemoryStream();

        void OnDataAvailable(object s, WaveInEventArgs e)
        {
            return;
            // audioStream에 데이터 추가
            if (e.Buffer.Length==0)
            {
                return;
            }

            var waveFormat = inputDevice.WaveFormat;
            var audioStreamMax = 0;
            if (audioStream.Length>0)
            {
                //audioStreamMax = audioStream.ToArray().Max();
                audioStreamMax = GetSamples(audioStream.ToArray(), waveFormat).Max();
            }

            /*
            var arr = e.Buffer;
            for (int i = 0;i< arr.Length;i++)
            {
                arr[i] = (byte)(i % 255);
            }
            audioStream.Write(arr, 0, e.BytesRecorded);

            */
            var cutout = 32;
            {

                var waveStream = new RawSourceWaveStream(new MemoryStream(e.Buffer), waveFormat) as WaveStream;
                waveViewer_Input.WaveStream = waveStream;

                if ((audioStreamMax > cutout) || (GetSamples(e.Buffer, waveFormat).Max() > cutout))
                {
                    audioStream.Write(e.Buffer, 0, e.BytesRecorded);
                }
            }

            if (audioStream.Length > inputDevice.WaveFormat.SampleRate*2)
            {
                var array = audioStream.ToArray();
                var samples = GetSamples(array, waveFormat);
                var startIndex = System.Array.FindIndex(samples, x => x > cutout);
                var lastIndex = System.Array.FindLastIndex(samples, x => x > cutout);
                var length = lastIndex - startIndex + 1;
                var sliceArray = array.Skip(startIndex * 2).Take(length * 2).ToArray();


                var max = (float)samples.Max();
                var amp = 32767f / max/2;
                amp = Math.Max(1, amp);
                amp = Math.Min(16, amp);
                sliceArray = Amplify(sliceArray, amp);
                //var stream = new RawSourceWaveStream(new MemoryStream((byte[])audioStream.ToArray().Clone()), waveFormat);
                var memoryStream = new MemoryStream(sliceArray);
                var waveStream = new RawSourceWaveStream(memoryStream, waveFormat) as WaveStream;

                waveViewer_Text.WaveStream = waveStream;
                //recognizer.SetInputToAudioStream(memoryStream, new SpeechAudioFormatInfo(waveFormat.SampleRate, AudioBitsPerSample.Sixteen, AudioChannel.Stereo));

                recognizer.SetInputToAudioStream(memoryStream, new SpeechAudioFormatInfo(
                    waveFormat.SampleRate,
                    (AudioBitsPerSample)waveFormat.BitsPerSample,
                    (AudioChannel)waveFormat.Channels
                ));

                //recognizer.SetInputToWaveStream(waveStream);
                recognizer.RecognizeAsync(RecognizeMode.Multiple);
                audioStream.SetLength(0);
                //Console.WriteLine("전달");
            }
        }
        byte[] Amplify(byte[] buffer, float gain)
        {
            var amplifiedBuffer = new byte[buffer.Length];
            for (int i = 0; i < buffer.Length; i += 2)
            {
                // 16비트 오디오 샘플 (리틀 엔디안)
                short sample = BitConverter.ToInt16(buffer, i);
                sample = (short)(sample * gain);

                // 클리핑 방지
                if (sample > short.MaxValue)
                    sample = short.MaxValue;
                else if (sample < short.MinValue)
                    sample = short.MinValue;

                var amplifiedSample = BitConverter.GetBytes(sample);
                amplifiedBuffer[i] = amplifiedSample[0];
                amplifiedBuffer[i + 1] = amplifiedSample[1];
            }
            return amplifiedBuffer;
        }

        short[] GetSamples(byte[] buffer, WaveFormat waveFormat)
        {
            int bytesPerSample = waveFormat.BitsPerSample / 8 * waveFormat.Channels;
            if (buffer.Length % bytesPerSample != 0)
                throw new ArgumentException("Buffer length is not a multiple of bytes per sample");

            int sampleCount = buffer.Length / bytesPerSample;
            short[] samples = new short[sampleCount];

            for (int i = 0; i < buffer.Length; i += bytesPerSample)
            {
                // PCM 데이터는 Little-Endian 형식
                samples[i / bytesPerSample] = BitConverter.ToInt16(buffer, i);
            }

            return samples;
        }

        void LoadRecognize()
        {
            Console.WriteLine("LoadRecognize()");
            var installedRecognizers = SpeechRecognitionEngine.InstalledRecognizers();
            foreach (var recognizerInfo in installedRecognizers)
            {
                Console.WriteLine(recognizerInfo.Culture);
            }

            var selectedCulture = installedRecognizers.FirstOrDefault();

            if (selectedCulture != null)
            {
                recognizer = new SpeechRecognitionEngine(selectedCulture);

                Console.WriteLine("세팅됨");
                recognizer.LoadGrammar(new DictationGrammar());

                recognizer.SpeechRecognized += (sender, e) =>
                {
                    if (textBox1_speech.Text!= e.Result.Text)
                    {
                        textBox1_speech.Text = e.Result.Text;
                        Speech(e.Result.Text);
                    }
                    Console.WriteLine("로드됨");
                };
                
                //recognizer.SetInputToDefaultAudioDevice();
                //recognizer.RecognizeAsync(RecognizeMode.Multiple);
                
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

            outputDevice.DeviceNumber = comboBox2_Output.SelectedIndex;
            /*
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
            */
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

        private void comboBox_Input_SelectedIndexChanged(object sender, EventArgs e)
        {
            inputDevice.StopRecording();
            inputDevice.DeviceNumber = comboBox_Input.SelectedIndex;
            inputDevice.StartRecording();
        }
    }
}

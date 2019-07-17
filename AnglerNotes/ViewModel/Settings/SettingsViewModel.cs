using System.Configuration;
using System.IO;

namespace AnglerNotes.ViewModel.Settings
{
    public class SettingsViewModel : ViewModelBase
    {
        private const int Width = 72;
        private const int Height = 33;

        /// <summary>
        /// Author's treat
        /// </summary>
        private const string FileContent = @"                                                                                                          ,<'_,_,,~.`;...                            ,'.                         ,+==*^<,<,'~,`~`,`-.    .:`--'':.           _2C`                        'IYv\v<-\:+,'~,~~-,-. .'^~'__~~~'..          _U{        .`--`,.          vOPs}!<!`\`<:*':^~`,^<_~<1*~_~^~~~.          .+1      '^^~^~~''`.       'KhhhPCT*1'**~~*~~+r**v/_^!1*~_^~:,            `\.   -*\>*/iIC}I(L{{V=^.1HkaKht%<_~~'^'~~\\\1*_=J\_*/1*^~~,             .\.  *rYItPHht{CjPhakkaPSfpEpt+^<+<+*+<**_*^+r+*+|r+^;~^~',              .!. =tUhpE]PtUe]HeXhekKdKHHK1v=||rri1!=\+*_~<r\_~_+v!*^_*.               ,?YXPf585HUXdDEHXPhkdE55DdHICtzPPPto{7|\\<__\1!+*~'~_**,              -ih2PAe&[gDOa56NpGffdNQgggg55sUPXhehhhPt7|!+*++<+\\\\*_~:             .7GdpOU2bBBNKObQ[g0ppdKd08BB$Q5PhkKdddKpdkIJ!<+Ic1\+\/*'.              ,lhPfaPHU55DpdfQBBgDED0Eddd08bDfOkE5bbbNDEa{\^,.`:,,..                 ^tUlUOhpkdXq55Dad[B65pE0bDpGadbEpEp]dDNN5Hah%*___~`.                   +UUPhStUf]dkDg9RhUh5gDO05NgbpfGpbNpKDbpddE5dP7\<*^':                   ~OUtGa]XI){CCCzUsUOHUp5XapN[[bpddpNgDdpN5Gd{r<^^~':.                   .PSedhXPeGK05NDhPPOd5dXdKDb[BB65DpppR[NEpbbkU\                          ^d5dhtUhAUsUdPCPPPXhXPUPKgBBB@BR50Epp5BgDED52U/                         +5pPtX}{PV@0dp/@{[NlkUPzUh6@@BB[650ppp5[QDpHdPT                         TpZtLCP8}j@@@X{@G@BomlAUtPObBBBBQ65EEppQBgEeOa\                         uPUeaC[@op@@@eP@@@@51bg]CtOp5B@BBQN5DEE6BB52UU,                         Imc@BP@@R#@@@QE@@@@QY50Ky{llpbBBB#g5DppbBBb2ty                          {5dNBB@B[[BB[gR8Mg[[kgDphTb{tOEBBB#RDEp5BBDP{~                          PUTloPBP56Nb0pPfGdppdEE0PlglUIUKBBBgb5DDBBHj~                           UP{XcT@%6NDpGKrHUf]GtGHaT2KhPhlWtNBB9N5EQ5U<                            +PUP(IB/Bh#8aprbY5bRYbdl}aUUAtstUl8BB9D]0P*                              cUPUIw?RY@6l]Lk=8H5ThICWUjPOeKD9Q[BQ5dXe*                               -FUUPsCU{K{PAPuUUPqXUUUPKbRbD0DDDD5DKPt_                                 ,\}e]aXUloUapE55pddpdf]dpppEpEpdkhUtV`                                     -!UXOkdDb8QQBQ8NDpdpp000EpdGUT<-.                                         .*CPPUPP2]KdpppKHfGK]kPT*:                                                 `>YmPeGK]fOXPUtzti_.                                                           .`~*<<*~`                                           ";
        
        /// <summary>
        /// Make sure to use the Consolas font or it won't look good
        /// </summary>
        public static string GetFileContent()
        {
            string output = "\r\n";
            for (int j = 0; j < Height; j++)
            {
                for (int i = 0; i < Width; i++)
                {
                    output += FileContent[i + j * Width];
                }
                output += "\r\n";
            }

            return output;
        }

        /// <summary>
        /// No comment on how useless this is, thanks
        /// </summary>
        public string Signature
        {
            get
            {
                return GetFileContent();
            }
        }

        /// <summary>
        /// Get C:\Users\USERNAME\AppData\Local\AnglerNotes\AnglerNotes.exe_Url_SUPERHASH\1.0.0.0
        /// </summary>
        public string FolderPath
        {
            get
            {
                string filepath = "";
                try
                {
                    var configUserLevel = ConfigurationUserLevel.PerUserRoamingAndLocal;
                    var UserConfig = ConfigurationManager.OpenExeConfiguration(configUserLevel);
                    filepath = UserConfig.FilePath;
                }
                catch (ConfigurationException e)
                {
                    filepath = e.Filename;
                }
                return "file://" + Path.GetDirectoryName(filepath);
            }
        }

        public SettingsViewModel()
        {
        }
    }
}

// Example usings.
using Celeste;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Celeste.Mod.TimerExport {
    [SettingName("modoptions_examplemodule_title")]
    public class TimerExportModuleSettings : EverestModuleSettings {

        [SettingName("File path")]
        [SettingMaxLength(500)]
        public string FilePath { get; set; } = "./H2Hraces.txt";
    }
}

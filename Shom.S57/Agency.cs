using System;

namespace S57
{
    public class Agency
    {
        internal Agency(uint code)
        {
            _code = code;
        }

        private uint _code;

        public uint Code
        {
            get
            {
                return _code;
            }
        }

        public string Name
        {
            get
            {
                switch (_code)
                {   //source: http://www.iho-ohi.net/s62/pdfExport/pacPDFExport.php
                    //retrieved on January 25, 2019

                    case 1: return "";
                    case 10: return "Australian Hydrographic Service (AHS)";
                    case 11: return "Australian Hydrographic Service - Navy and Defence";
                    case 20: return "Hydrographic Survey Office";
                    case 30: return "MDK – Afdeling Kust – Division Coast";
                    case 40: return "Directorate of Hydrography and Navigation (DHN)";
                    case 50: return "Canadian Hydrographic Service (CHS)";
                    case 51: return "Canadian Forces";
                    case 60: return "Servicio Hidrográfico y Oceanográfico de la Armada (SHOA)";
                    case 70: return "Maritime Safety Administration (MSA)";
                    case 71: return "The Navigation Guarantee Department of The Chinese Navy Headquarters";
                    case 72: return "Hong Kong Special Administrative Region";
                    case 73: return "Macau Special Administrative Region";
                    case 80: return "Hrvatski Hidrografski Institut";
                    case 90: return "Oficina Nacional de Hidrografia y Geodesia";
                    case 100: return "Hydrographic Unit of the Department of Lands and Surveys";
                    case 110: return "Geodatastyrelsen (GST)";
                    case 120: return "Instituto Cartografico Militar";
                    case 130: return "Instituto Oceanográfico de la Armada (INOCAR)";
                    case 140: return "Shobat al Misaha al Baharia";
                    case 150: return "Fiji Islands Maritime Safety Administration (FIMSA)";
                    case 160: return "Finnish Transport Agency (FTA)";
                    case 170: return "Service Hydrographique et Océanographique de la Marine (SHOM)";
                    case 180: return "Bundesamt für Seeschiffahrt und Hydrographie (BSH)";
                    case 190: return "Hellenic Navy Hydrographic Service (HNHS)";
                    case 200: return "Ministerio de la Defensa Nacional";
                    case 201: return "Comisión Portuaria Nacional";
                    case 210: return "Icelandic Coast Guard";
                    case 220: return "National Hydrographic Office";
                    case 221: return "Indian Navy Specific";
                    case 230: return "Jawatan Hidro-Oseanografi (JANHIDROS)";
                    case 240: return "Ports and Shipping Organization (PSO)";
                    case 250: return "Istituto Idrografico della Marina (IIM)";
                    case 260: return "Japan Hydrographic and Oceanographic Department (JHOD)";
                    case 270: return "Hydrographic Department";
                    case 280: return "Korea Hydrographic and Oceanographic Agency (KHOA)";
                    case 290: return "National Hydrographic Centre";
                    case 300: return "Direction des Affaires Maritimes";
                    case 310: return "Koninklijke Marine Dienst der Hydrografie / CZSK";
                    case 320: return "Land Information New Zealand (LINZ)";
                    case 321: return "Geospatial Intelligence New Zealand";
                    case 330: return "Nigerian Navy Hydrographic Office";
                    case 340: return "Norwegian Hydrographic Service";
                    case 341: return "Electronic Chart Centre";
                    case 342: return "Norwegian Defence";
                    case 350: return "National Hydrographic Office";
                    case 360: return "Pakistan Hydrographic Department";
                    case 370: return "Hydrographic Division, National Maritime Safety Division (NMSA)";
                    case 380: return "Dirección de Hidrografía y Navegación (DHN)";
                    case 390: return "National Mapping and Resource Information Authority, Coast & Geodetic Survey Department";
                    case 400: return "Biuro Hydrograficzne";
                    case 401: return "Inland Navigation Office in Szczecin (Urzad Zeglugi Srodladowej w Szczecinie)";
                    case 410: return "Instituto Hidrografico, Portugal (IHP)";
                    case 420: return "Head Department of Navigation & Oceanography (DNO)";
                    case 425: return "Federal State Unitary Hydrographc Department";
                    case 430: return "Hydrographic Department, Maritime and Port Authority (MPA)";
                    case 440: return "South African Navy Hydrographic Office (SANHO)";
                    case 450: return "Instituto Hidrográfico de la Marina (IHM)";
                    case 460: return "National Hydrographic Office, National Aquatic Resources Research and Development Agency (NARA)";
                    case 470: return "Maritieme Autoriteit Suriname (MAS)";
                    case 480: return "Sjöfartsverket, Swedish Maritime Administration";
                    case 490: return "General Directorate of Ports";
                    case 500: return "Hydrographic Department, Royal Thai Navy";
                    case 505: return "Tonga Defence Services";
                    case 510: return "Trinidad & Tobago Hydrographic Unit";
                    case 520: return "Hydrography and Oceanography";
                    case 530: return "Ministry of Communications";
                    case 540: return "United Kingdom Hydrographic Office";
                    case 550: return "Office of Coast Survey, National Ocean Service, National Oceanic and Atmospheric Administration (NOS)";
                    case 551: return "National Geospatial-Intelligence Agency Department of Defense (NGA)";
                    case 552: return "Commander, Naval Meteorology and Oceanography Command (CNMOC)";
                    case 553: return "U.S. Army Corps of Engineers (USACE)";
                    case 560: return "";
                    case 570: return "Commandancia General de la Armada, Dirección de Hidrografía y Navegación (DHN)";
                    case 580: return "Direkcija Za Unutrašnje Plovne Puteve";
                    case 590: return "Ministère des Transports et Communications";
                    case 660: return "Hydrographic Department";
                    case 710: return "Department of Marine";
                    case 715: return "Survey Department";
                    case 740: return "Port Autonome de Douala (PAD)";
                    case 760: return "Ministerio de Defensa Nacional";
                    case 870: return "Estonian Maritime Administration (EMA)";
                    case 905: return "State Hydrographic Service of Georgia";
                    case 990: return "Maritime Safety Directorate";
                    case 1010: return "Surveys and Mapping Division";
                    case 1050: return "Ministry of Communications";
                    case 1060: return "Maritime Administration of Latvia";
                    case 1140: return "Malta Maritime Authority Ports Directorate, Hydrographic Unit";
                    case 1170: return "Ministry of Housing and Land, Hydrographic Unit";
                    case 1180: return "Secretaria de Marina – Armada de Mexico, Direccion General Adjunta de Oceanografia, Hidrografia y Meteorologia";
                    case 1200: return "Division Hydrographie et Cartographie (DHC) de la Marine Royale";
                    case 1210: return "Instituto Nacional de Hidrografia e Navegação (INAHINA)";
                    case 1220: return "Central Naval Hydrographic Depot (CNHD)";
                    case 1225: return "Ministry of Defence, Navy Headquarters";
                    case 1226: return "Institute of Hydrometeorology and Seismology";
                    case 1290: return "Urban Planning & Development Authority, Hydrographic Section";
                    case 1300: return "Directia Hidrografica Maritima";
                    case 1360: return "General Directorate of Military Survey (GDMS)";
                    case 1365: return "General Commission for Survey (GCS)";
                    case 1400: return "Ministry of Transport Maritime Office";
                    case 1470: return "Service Hydrographique et Océanographique (SHO), Armée de Mer Seyir, Hidrografi ve Osinografi Dairesi Baskanligi, Office of Navigation,";
                    case 1490: return "State Hydrographic Service of Ukraine";
                    case 1510: return "Viet Nam Maritime People's Navy";
                    case 1511: return "Viet Nam Maritime Safety-North (VMS-N)";
                    case 1512: return "Viet Nam Maritime Safety-South (VMS-S)";
                }

                throw new NotImplementedException("Producing Agencies");
            }
        }
    }
}

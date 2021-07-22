using CowinWatcher.Entities;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace CowinWatcher
{
    public class ViewModel : BaseViewModel
    {
        Business service;

        private List<CenterInfo> _slotLists;
        public string[] SessionDates { get; set; }
        private StateData _statesInfo;
        private DistrictData _districtsInfo;
        private District _districtSelected;
        private State _stateSelected;

        private int timer_refreshTimer;

        public int RefreshTimer
        {
            get { return timer_refreshTimer; }
            set { timer_refreshTimer = value; OnPropertyChanged(); }
        }

        public District DistrictSelected
        {
            get { return _districtSelected; }
            set { _districtSelected = value; LoadMasterInfoUsingDistrict(StateSelected.state_name, DistrictSelected.district_name); OnPropertyChanged(); }
        }

        public State StateSelected
        {
            get { return _stateSelected; }
            set { _stateSelected = value; RefreshDistrict(_stateSelected.state_id.ToString()); OnPropertyChanged(); }
        }

        private void RefreshDistrict(string state_id)
        {
            DistrictsInfo = service.GetDistricts(state_id);
            DistrictSelected = DistrictsInfo.districts.FirstOrDefault();
        }

        public List<CenterInfo> SlotLists
        {
            get { return _slotLists; }
            set { _slotLists = value; RefreshTimer = 10; OnPropertyChanged(); }
        }

        public StateData StatesInfo
        {
            get { return _statesInfo; }
            set
            {
                _statesInfo = value; OnPropertyChanged();
            }
        }

        public DistrictData DistrictsInfo
        {
            get { return _districtsInfo; }
            set { _districtsInfo = value; DistrictSelected = DistrictsInfo.districts[0]; OnPropertyChanged(); }
        }

        private DispatcherTimer _timer;
        SlackManager _manager;

        public List<CenterInfo> temp_slots { get; set; }

        public ViewModel()
        {
            service = new Business();
            StatesInfo = service.GetStates();
            StateSelected = StatesInfo.states[0];
            LoadSessionDates();
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += RefreshTimer_Tick;
            _timer.Start();

            _manager = new SlackManager();

        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            if (RefreshTimer == 0)
            {
                temp_slots = SlotLists;
                LoadMasterInfoUsingDistrict(StateSelected.state_name, DistrictSelected.district_name);
                if (temp_slots.Count != 0) CheckForNewOpenSlots();
            }
            else
            {
                RefreshTimer = RefreshTimer - 1;
            }

        }

        public void LoadMasterInfoUsingDistrict(string state, string district)
        {
            if (StateSelected == null)
            {
                StateSelected = StatesInfo.states.Where(p => p.state_name.ToUpper() == state.ToUpper()).First();

                DistrictSelected = DistrictsInfo.districts.Where(p => p.district_name.ToUpper() == district.ToUpper()).First();
            }


            List<Center> Centers = new Business().GetMasterDataForDistrict(DistrictSelected.district_id.ToString()).centers;
            if (Centers.Count == 0)
            {
                DistrictSelected = DistrictsInfo.districts[1];
            }
            SlotLists = new List<CenterInfo>();

            foreach (Center center in Centers)
            {
                var session = GetSessionsForCenter(center);
                SlotLists.Add(session);
            }

        }
        public void LoadSessionDates()
        {
            SessionDates = new string[] { DateTime.Now.ToString("dd-MM-yyyy"), DateTime.Now.AddDays(1).ToString("dd-MM-yyyy"), DateTime.Now.AddDays(2).ToString("dd-MM-yyyy") };
        }
        public static CenterInfo GetSessionsForCenter(Center data)
        {
            List<Session> sessions;
            CenterInfo center = new CenterInfo();
            center.SessionDay1 = center.SessionDay2 = center.SessionDay3 = center.SessionDay4 = center.SessionDay5 = center.SessionDay6 = new List<Session>();
            try
            {
                for (int i = 1; i < 7; i++)
                {
                    sessions = new List<Session>();
                    var sessionOndate = data.sessions.Where(p => p.date == DateTime.Now.AddDays(i - 1).ToString("dd-MM-yyyy"));
                    if (sessionOndate.Count() != 0)
                        sessions = sessions.Concat(sessionOndate).ToList();
                    else
                        sessions.Add(new Session());

                    //var t =(List<Session>)Convert.ChangeType( center.GetType().GetProperty("SessionDay" + (i + 1)).PropertyType, typeof(List<Session>));
                    //t.Concat(sessionOndate);
                    switch (i)
                    {
                        case 1:
                            center.SessionDay1 = center.SessionDay1.Concat(sessions).ToList();
                            break;
                        case 2:
                            center.SessionDay2 = center.SessionDay2.Concat(sessions).ToList();
                            break;
                        case 3:
                            center.SessionDay3 = center.SessionDay3.Concat(sessions).ToList();
                            break;
                        case 4:
                            center.SessionDay4 = center.SessionDay4.Concat(sessions).ToList();
                            break;
                        case 5:
                            center.SessionDay5 = center.SessionDay5.Concat(sessions).ToList();
                            break;
                        case 6:
                            center.SessionDay6 = center.SessionDay6.Concat(sessions).ToList();
                            break;
                        default:
                            break;
                    }

                }
                center.CenterDetail = data;

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return center;
        }

        public void CheckForNewOpenSlots()
        {
            List<string> lstSessionIds = new List<string>();
            List<string> lstnewSessionsIds = new List<string>();

            temp_slots = (from p in temp_slots
                          from q in p.SessionDay1
                          where q.available_capacity != 0
                          select p).ToList();



            var v = (from p in SlotLists
                     from q in p.SessionDay1
                     where q.available_capacity != 0
                     select p).ToList();
            temp_slots[1].SessionDay1.RemoveAt(0);

            var newslots = from p in temp_slots
                           from q in v
                           from r in q.SessionDay1
                           where p.SessionDay1.IndexOf(r) == -1
                           select q;


            temp_slots.ForEach(p => p.SessionDay1.ForEach(q => lstSessionIds.Add(q.session_id)));
            SlotLists.ForEach(p => p.SessionDay1.ForEach(q => lstnewSessionsIds.Add(q.session_id)));
            //lstSessionIds.RemoveAt(new Random().Next(0,lstSessionIds.Count-1));
            //var newslots = lstnewSessionsIds.Except(lstSessionIds);
            foreach (var session in newslots)
            {
                session.SessionDay1.ForEach(q => NotifyForNewSlots(q, session.CenterDetail));
            }
        }

        private void NotifyForNewSlots(Session sess, Center center)
        {

            string msg = sess.vaccine + "\nAge: " + sess.min_age_limit + "\nDose 1:" + sess.available_capacity_dose1 + "\nDose 2:" + sess.available_capacity_dose2 + "\nCenter:" + center.name;
            _manager.SendMessage(msg);

        }
    }

}

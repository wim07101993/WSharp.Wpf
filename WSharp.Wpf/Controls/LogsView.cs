using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

using WSharp.Extensions;
using WSharp.Logging;
using WSharp.Wpf.Extensions;

namespace WSharp.Wpf.Controls
{
    [TemplatePart(Name = nameof(PartSearchButton), Type = typeof(Button))]
    [TemplatePart(Name = nameof(PartClearFilterButton), Type = typeof(Button))]
    public class LogsView : ACollectionControl<ILogEntry>
    {
        private const TraceEventType NoEventTypeFilter =
            TraceEventType.Critical |
            TraceEventType.Error |
            TraceEventType.Warning |
            TraceEventType.Information |
            TraceEventType.Verbose |
            TraceEventType.Start |
            TraceEventType.Stop |
            TraceEventType.Suspend |
            TraceEventType.Resume |
            TraceEventType.Transfer;

        static LogsView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(LogsView), new FrameworkPropertyMetadata(typeof(LogsView)));
        }

        #region collumn visibilities

        public static readonly DependencyProperty IdColumnVisibilityProperty = DependencyProperty.Register(nameof(IdColumnVisibility), typeof(Visibility), typeof(LogsView), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty SourceColumnVisibilityProperty = DependencyProperty.Register(nameof(SourceColumnVisibility), typeof(Visibility), typeof(LogsView), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty TimeColumnVisibilityProperty = DependencyProperty.Register(nameof(TimeColumnVisibility), typeof(Visibility), typeof(LogsView), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty TagColumnVisibilityProperty = DependencyProperty.Register(nameof(TagColumnVisibility), typeof(Visibility), typeof(LogsView), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty EventTypeColumnVisibilityProperty = DependencyProperty.Register(nameof(EventTypeColumnVisibility), typeof(Visibility), typeof(LogsView), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty TitleColumnVisibilityProperty = DependencyProperty.Register(nameof(TitleColumnVisibility), typeof(Visibility), typeof(LogsView), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty PayloadColumnVisibilityProperty = DependencyProperty.Register(nameof(PayloadColumnVisibility), typeof(Visibility), typeof(LogsView), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty OperationStackColumnVisibilityProperty = DependencyProperty.Register(nameof(OperationStackColumnVisibility), typeof(Visibility), typeof(LogsView), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty TimeStampColumnVisibilityProperty = DependencyProperty.Register(nameof(TimeStampColumnVisibility), typeof(Visibility), typeof(LogsView), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty ProcessIdColumnVisibilityProperty = DependencyProperty.Register(nameof(ProcessIdColumnVisibility), typeof(Visibility), typeof(LogsView), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty ThreadIdColumnVisibilityProperty = DependencyProperty.Register(nameof(ThreadIdColumnVisibility), typeof(Visibility), typeof(LogsView), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty CallStackColumnVisibilityProperty = DependencyProperty.Register(nameof(CallStackColumnVisibility), typeof(Visibility), typeof(LogsView), new PropertyMetadata(Visibility.Collapsed));

        public Visibility IdColumnVisibility
        {
            get => (Visibility)GetValue(IdColumnVisibilityProperty);
            set => SetValue(IdColumnVisibilityProperty, value);
        }

        public Visibility SourceColumnVisibility
        {
            get => (Visibility)GetValue(SourceColumnVisibilityProperty);
            set => SetValue(SourceColumnVisibilityProperty, value);
        }

        public Visibility TimeColumnVisibility
        {
            get => (Visibility)GetValue(TimeColumnVisibilityProperty);
            set => SetValue(TimeColumnVisibilityProperty, value);
        }

        public Visibility TagColumnVisibility
        {
            get => (Visibility)GetValue(TagColumnVisibilityProperty);
            set => SetValue(TagColumnVisibilityProperty, value);
        }

        public Visibility EventTypeColumnVisibility
        {
            get => (Visibility)GetValue(EventTypeColumnVisibilityProperty);
            set => SetValue(EventTypeColumnVisibilityProperty, value);
        }

        public Visibility TitleColumnVisibility
        {
            get => (Visibility)GetValue(TitleColumnVisibilityProperty);
            set => SetValue(TitleColumnVisibilityProperty, value);
        }

        public Visibility PayloadColumnVisibility
        {
            get => (Visibility)GetValue(PayloadColumnVisibilityProperty);
            set => SetValue(PayloadColumnVisibilityProperty, value);
        }

        public Visibility OperationStackColumnVisibility
        {
            get => (Visibility)GetValue(OperationStackColumnVisibilityProperty);
            set => SetValue(OperationStackColumnVisibilityProperty, value);
        }

        public Visibility TimeStampColumnVisibility
        {
            get => (Visibility)GetValue(TimeStampColumnVisibilityProperty);
            set => SetValue(TimeStampColumnVisibilityProperty, value);
        }

        public Visibility ProcessIdColumnVisibility
        {
            get => (Visibility)GetValue(ProcessIdColumnVisibilityProperty);
            set => SetValue(ProcessIdColumnVisibilityProperty, value);
        }

        public Visibility ThreadIdColumnVisibility
        {
            get => (Visibility)GetValue(ThreadIdColumnVisibilityProperty);
            set => SetValue(ThreadIdColumnVisibilityProperty, value);
        }

        public Visibility CallStackColumnVisibility
        {
            get => (Visibility)GetValue(CallStackColumnVisibilityProperty);
            set => SetValue(CallStackColumnVisibilityProperty, value);
        }

        #endregion collumn visibilities

        #region filter

        public static readonly DependencyProperty IdFilterProperty = DependencyProperty.Register(nameof(IdFilter), typeof(string), typeof(LogsView));
        public static readonly DependencyProperty SourceFilterProperty = DependencyProperty.Register(nameof(SourceFilter), typeof(string), typeof(LogsView));
        public static readonly DependencyProperty TagFilterProperty = DependencyProperty.Register(nameof(TagFilter), typeof(string), typeof(LogsView));
        public static readonly DependencyProperty EventTypeFilterProperty = DependencyProperty.Register(nameof(EventTypeFilter), typeof(TraceEventType), typeof(LogsView), new PropertyMetadata(NoEventTypeFilter));
        public static readonly DependencyProperty TitleFilterProperty = DependencyProperty.Register(nameof(TitleFilter), typeof(string), typeof(LogsView));
        public static readonly DependencyProperty PayloadFilterProperty = DependencyProperty.Register(nameof(PayloadFilter), typeof(string), typeof(LogsView));
        public static readonly DependencyProperty OperationStackFilterProperty = DependencyProperty.Register(nameof(OperationStackFilter), typeof(string), typeof(LogsView));
        public static readonly DependencyProperty DateLowerLimitProperty = DependencyProperty.Register(nameof(DateLowerLimit), typeof(DateTime?), typeof(LogsView));
        public static readonly DependencyProperty TimeLowerLimitProperty = DependencyProperty.Register(nameof(TimeLowerLimit), typeof(DateTime?), typeof(LogsView));
        public static readonly DependencyProperty DateUpperLimitProperty = DependencyProperty.Register(nameof(DateUpperLimit), typeof(DateTime?), typeof(LogsView));
        public static readonly DependencyProperty TimeUpperLimitProperty = DependencyProperty.Register(nameof(TimeUpperLimit), typeof(DateTime?), typeof(LogsView));
        public static readonly DependencyProperty TimeStampLowerLimitProperty = DependencyProperty.Register(nameof(TimeStampLowerLimit), typeof(int?), typeof(LogsView));
        public static readonly DependencyProperty TimeStampUpperLimitProperty = DependencyProperty.Register(nameof(TimeStampUpperLimit), typeof(int?), typeof(LogsView));
        public static readonly DependencyProperty ProcessIdFilterProperty = DependencyProperty.Register(nameof(ProcessIdFilter), typeof(int?), typeof(LogsView));
        public static readonly DependencyProperty ThreadIdFilterProperty = DependencyProperty.Register(nameof(ThreadIdFilter), typeof(string), typeof(LogsView));
        public static readonly DependencyProperty CallStackFilterProperty = DependencyProperty.Register(nameof(CallStackFilter), typeof(string), typeof(LogsView));

        public string IdFilter
        {
            get => (string)GetValue(IdFilterProperty);
            set => SetValue(IdFilterProperty, value);
        }

        public string SourceFilter
        {
            get => (string)GetValue(SourceFilterProperty);
            set => SetValue(SourceFilterProperty, value);
        }

        public string TagFilter
        {
            get => (string)GetValue(TagFilterProperty);
            set => SetValue(TagFilterProperty, value);
        }

        public TraceEventType EventTypeFilter
        {
            get => (TraceEventType)GetValue(EventTypeFilterProperty);
            set => SetValue(EventTypeFilterProperty, value);
        }

        public string TitleFilter
        {
            get => (string)GetValue(TitleFilterProperty);
            set => SetValue(TitleFilterProperty, value);
        }

        public string PayloadFilter
        {
            get => (string)GetValue(PayloadFilterProperty);
            set => SetValue(PayloadFilterProperty, value);
        }

        public string OperationStackFilter
        {
            get => (string)GetValue(OperationStackFilterProperty);
            set => SetValue(OperationStackFilterProperty, value);
        }

        public DateTime? TimeLowerLimit
        {
            get => (DateTime?)GetValue(TimeLowerLimitProperty);
            set => SetValue(TimeLowerLimitProperty, value);
        }

        public DateTime? DateLowerLimit
        {
            get => (DateTime?)GetValue(DateLowerLimitProperty);
            set => SetValue(DateLowerLimitProperty, value);
        }

        public DateTime? TimeUpperLimit
        {
            get => (DateTime?)GetValue(TimeUpperLimitProperty);
            set => SetValue(TimeUpperLimitProperty, value);
        }

        public DateTime? DateUpperLimit
        {
            get => (DateTime?)GetValue(DateUpperLimitProperty);
            set => SetValue(DateUpperLimitProperty, value);
        }

        public int? TimeStampLowerLimit
        {
            get => (int?)GetValue(TimeStampLowerLimitProperty);
            set => SetValue(TimeStampLowerLimitProperty, value);
        }

        public int? TimeStampUpperLimit
        {
            get => (int?)GetValue(TimeStampUpperLimitProperty);
            set => SetValue(TimeStampUpperLimitProperty, value);
        }

        public int? ProcessIdFilter
        {
            get => (int?)GetValue(ProcessIdFilterProperty);
            set => SetValue(ProcessIdFilterProperty, value);
        }

        public string ThreadIdFilter
        {
            get => (string)GetValue(ThreadIdFilterProperty);
            set => SetValue(ThreadIdFilterProperty, value);
        }

        public string CallStackFilter
        {
            get => (string)GetValue(CallStackFilterProperty);
            set => SetValue(CallStackFilterProperty, value);
        }

        #endregion filter

        #region filter visibilities

        public static readonly DependencyProperty IdFilterVisibilityProperty = DependencyProperty.Register(nameof(IdFilterVisibility), typeof(Visibility), typeof(LogsView), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty SourceFilterVisibilityProperty = DependencyProperty.Register(nameof(SourceFilterVisibility), typeof(Visibility), typeof(LogsView), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty TimeFilterVisibilityProperty = DependencyProperty.Register(nameof(TimeFilterVisibility), typeof(Visibility), typeof(LogsView), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty TagFilterVisibilityProperty = DependencyProperty.Register(nameof(TagFilterVisibility), typeof(Visibility), typeof(LogsView), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty EventTypeFilterVisibilityProperty = DependencyProperty.Register(nameof(EventTypeFilterVisibility), typeof(Visibility), typeof(LogsView), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty TitleFilterVisibilityProperty = DependencyProperty.Register(nameof(TitleFilterVisibility), typeof(Visibility), typeof(LogsView), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty PayloadFilterVisibilityProperty = DependencyProperty.Register(nameof(PayloadFilterVisibility), typeof(Visibility), typeof(LogsView), new PropertyMetadata(Visibility.Visible));
        public static readonly DependencyProperty OperationStackFilterVisibilityProperty = DependencyProperty.Register(nameof(OperationStackFilterVisibility), typeof(Visibility), typeof(LogsView), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty TimeStampFilterVisibilityProperty = DependencyProperty.Register(nameof(TimeStampFilterVisibility), typeof(Visibility), typeof(LogsView), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty ProcessIdFilterVisibilityProperty = DependencyProperty.Register(nameof(ProcessIdFilterVisibility), typeof(Visibility), typeof(LogsView), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty ThreadIdFilterVisibilityProperty = DependencyProperty.Register(nameof(ThreadIdFilterVisibility), typeof(Visibility), typeof(LogsView), new PropertyMetadata(Visibility.Collapsed));
        public static readonly DependencyProperty CallStackFilterVisibilityProperty = DependencyProperty.Register(nameof(CallStackFilterVisibility), typeof(Visibility), typeof(LogsView), new PropertyMetadata(Visibility.Collapsed));

        public Visibility IdFilterVisibility
        {
            get => (Visibility)GetValue(IdFilterVisibilityProperty);
            set => SetValue(IdFilterVisibilityProperty, value);
        }

        public Visibility SourceFilterVisibility
        {
            get => (Visibility)GetValue(SourceFilterVisibilityProperty);
            set => SetValue(SourceFilterVisibilityProperty, value);
        }

        public Visibility TimeFilterVisibility
        {
            get => (Visibility)GetValue(TimeFilterVisibilityProperty);
            set => SetValue(TimeFilterVisibilityProperty, value);
        }

        public Visibility TagFilterVisibility
        {
            get => (Visibility)GetValue(TagFilterVisibilityProperty);
            set => SetValue(TagFilterVisibilityProperty, value);
        }

        public Visibility EventTypeFilterVisibility
        {
            get => (Visibility)GetValue(EventTypeFilterVisibilityProperty);
            set => SetValue(EventTypeFilterVisibilityProperty, value);
        }

        public Visibility TitleFilterVisibility
        {
            get => (Visibility)GetValue(TitleFilterVisibilityProperty);
            set => SetValue(TitleFilterVisibilityProperty, value);
        }

        public Visibility PayloadFilterVisibility
        {
            get => (Visibility)GetValue(PayloadFilterVisibilityProperty);
            set => SetValue(PayloadFilterVisibilityProperty, value);
        }

        public Visibility OperationStackFilterVisibility
        {
            get => (Visibility)GetValue(OperationStackFilterVisibilityProperty);
            set => SetValue(OperationStackFilterVisibilityProperty, value);
        }

        public Visibility TimeStampFilterVisibility
        {
            get => (Visibility)GetValue(TimeStampFilterVisibilityProperty);
            set => SetValue(TimeStampFilterVisibilityProperty, value);
        }

        public Visibility ProcessIdFilterVisibility
        {
            get => (Visibility)GetValue(ProcessIdFilterVisibilityProperty);
            set => SetValue(ProcessIdFilterVisibilityProperty, value);
        }

        public Visibility ThreadIdFilterVisibility
        {
            get => (Visibility)GetValue(ThreadIdFilterVisibilityProperty);
            set => SetValue(ThreadIdFilterVisibilityProperty, value);
        }

        public Visibility CallStackFilterVisibility
        {
            get => (Visibility)GetValue(CallStackFilterVisibilityProperty);
            set => SetValue(CallStackFilterVisibilityProperty, value);
        }

        #endregion filter visibilities

        #region METHODS

        public override void ClearFilter()
        {
            IdFilter = null;
            SourceFilter = null;
            TagFilter = null;
            TitleFilter = null;
            ThreadIdFilter = null;
            CallStackFilter = null;
            PayloadFilter = null;
            OperationStackFilter = null;
            TimeStampUpperLimit = null;
            TimeStampLowerLimit = null;
            EventTypeFilter = NoEventTypeFilter;
            ProcessIdFilter = null;
            DateLowerLimit = null;
            TimeLowerLimit = null;
            DateUpperLimit = null;
            DateLowerLimit = null;

            UpdateFilteredItemsSource();
        }

        public override IEnumerable<ILogEntry> FilterItems(IEnumerable<ILogEntry> items)
        {
            items = items
                .WhereStringContains(x => x.Id.ToString(), IdFilter)
                .WhereStringContains(x => x.Source, SourceFilter)
                .WhereStringContains(x => x.Tag, TagFilter)
                .WhereStringContains(x => x.Title, TitleFilter)
                .WhereStringContains(x => x.EventCache?.ThreadId, ThreadIdFilter)
                .WhereStringContains(x => x.EventCache?.Callstack, CallStackFilter)
                .WhereCollectionContainsString(x => x.Payload, PayloadFilter)
                .WhereCollectionContainsString(x => x.EventCache?.LogicalOperationStack, OperationStackFilter)
                .WhereBetweenOrEqual(x => x.EventCache?.Timestamp, TimeStampUpperLimit, TimeStampLowerLimit);

            if (ProcessIdFilter != null)
                items = items.Where(x => x.EventCache?.ProcessId == ProcessIdFilter);

            if (DateLowerLimit != null)
                items = items.WhereGreaterOrEqual(x => x.EventCache.DateTime, DateLowerLimit?.ChangeTime(TimeLowerLimit));
            else if (TimeLowerLimit != null)
                items = items.WhereGreaterOrEqual(x => x.EventCache?.DateTime.TimeOfDay, TimeLowerLimit?.TimeOfDay);

            if (DateUpperLimit != null)
                items = items.WhereGreaterOrEqual(x => x.EventCache.DateTime, DateUpperLimit?.ChangeTime(TimeUpperLimit));
            else if (TimeUpperLimit != null)
                items = items.WhereGreaterOrEqual(x => x.EventCache?.DateTime.TimeOfDay, TimeUpperLimit?.TimeOfDay);

#pragma warning disable RECS0016 // Bitwise operation on enum which has no [Flags] attribute
            if (EventTypeFilter != NoEventTypeFilter)
                items = items.Where(x => (EventTypeFilter & x.EventType) > 0);
#pragma warning restore RECS0016 // Bitwise operation on enum which has no [Flags] attribute

            return items;
        }

        #endregion METHODS
    }
}

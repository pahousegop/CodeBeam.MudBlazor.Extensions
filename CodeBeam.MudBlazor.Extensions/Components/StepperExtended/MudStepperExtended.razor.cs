﻿using MudExtensions.Utilities;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Extensions;
using MudBlazor.Utilities;

namespace MudExtensions
{
    /// <summary>
    /// Stepper component with extended features.
    /// </summary>
    public partial class MudStepperExtended : MudComponentBase
    {
        MudAnimate _animate = new();
        Guid _animateGuid = Guid.NewGuid();

        /// <summary>
        /// 
        /// </summary>
        protected string? HeaderClassname => new CssBuilder("d-flex align-center mud-stepper-header gap-4 pa-3")
            .AddClass("mud-ripple", Ripple && !Linear)
            .AddClass("cursor-pointer mud-stepper-header-non-linear", !Linear)
            .AddClass("flex-column", !Vertical)
            .AddClass("flex-row", Vertical)
            .Build();

        /// <summary>
        /// 
        /// </summary>
        protected string? ContentClassname => new CssBuilder($"mud-stepper-ani-{_animateGuid.ToString()}")
            .AddClass(ContentClass)
            .Build();

        /// <summary>
        /// 
        /// </summary>
        protected string? ActionClassname => new CssBuilder("d-flex gap-4")
            .AddClass(ActionClass)
            .Build();

        /// <summary>
        /// 
        /// </summary>
        protected string? AvatarStylename => new StyleBuilder()
            .AddStyle("z-index: 20")
            .AddStyle("background-color", "var(--mud-palette-background)", Variant == Variant.Outlined)
            .Build();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string? GetMobileStyle()
        {
            if(Vertical)
            {
                return "grid-column:1;margin-inline-start:22px;";
            }
            else
            {
                return "grid-row:1;margin-top:22px;";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string? GetStepperStyle()
        {
            var count = Steps.Count * 2;
            if (Vertical)
            {
                return $"display:grid;grid-template-rows:repeat({count}, 1fr);";
            }
            else
            {
                return $"display:grid;grid-template-columns:repeat({count}, 1fr);";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string? GetStepperSubStyle()
        {
            if (Vertical)
            {
                return "grid-row-start:1;grid-row-end:-1;flex-direction:column;grid-column:1;list-style:none;display:flex;";
            }
            else
            {
                return "grid-column-start:1;grid-column-end:-1;flex-direction:row;grid-row:1;list-style:none;display:flex;";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string? GetStepPercent()
        {
            var dPercent = (100.0 / Steps.Count).ToInvariantString();
            if (Vertical)
            {
                return $"height:{dPercent}%";
            }
            else
            {
                return $"width:{dPercent}%";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string? GetStepClass()
        {
            if (Vertical)
            {
                return $"d-flex";
            }
            else
            {
                return $"";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string? GetProgressLinearStyle()
        {
            var end = Steps.Count * 2;
            if (Vertical)
            {
                return $"grid-row-start:2;grid-row-end:{end};grid-column:1/-1;display:inline-grid;left:{(HeaderSize == Size.Medium ? 30 : HeaderSize == Size.Large ? 38 : 22)}px;z-index:10;transform:rotateX(180deg);";
            }
            else
            {
                return $"grid-column-start:2;grid-column-end:{end};grid-row:1/-1;display:inline-grid;top:{(HeaderSize == Size.Medium ? 30 : HeaderSize == Size.Large ? 38 : 22)}px;{(HeaderSize == Size.Small ? "height:2px;" : HeaderSize == Size.Medium ? "height:3px;" : null)}{(MobileView ? "margin-inline-start:40px;" : null)}z-index:10";
            }
        }

        private int _activeIndex;
        internal int ActiveIndex
        {
            get => _activeIndex;
            set
            {
                _activeIndex = value;
                UpdateProgressValue();
            }
        }

        internal double ProgressValue;
        /// <summary>
        /// 
        /// </summary>
        protected void UpdateProgressValue()
        {
            ProgressValue = _activeIndex * (100.0 / (Steps.Count - 1));
        }

        /// <summary>
        /// Provides CSS classes for the step content.
        /// </summary>
        [Parameter]
        public string? ContentClass { get; set; }

        /// <summary>
        /// Provides CSS styles for the step content.
        /// </summary>
        [Parameter]
        public string? ContentStyle { get; set; }

        /// <summary>
        /// Provides CSS classes for the step actions.
        /// </summary>
        [Parameter]
        public string? ActionClass { get; set; }

        /// <summary>
        /// Determines how action buttons justified.
        /// </summary>
        [Parameter]
        public StepperActionsJustify StepperActionsJustify { get; set; }

        /// <summary>
        /// If true, the header can not be clickable and users can step one by one.
        /// </summary>
        [Parameter]
        public bool Linear { get; set; }

        /// <summary>
        /// If true, disables ripple effect when click on step headers.
        /// </summary>
        [Parameter]
        public bool Ripple { get; set; } = true;

        /// <summary>
        /// If true, disables the default animation on step changing.
        /// </summary>
        [Parameter]
        public bool Animation { get; set; } = true;

        /// <summary>
        /// If true, disables built-in "previous" step action button.
        /// </summary>
        [Parameter]
        public bool ShowPreviousButton { get; set; } = true;

        /// <summary>
        /// If true, disables built-in "next" step action button.
        /// </summary>
        [Parameter]
        public bool ShowNextButton { get; set; } = true;

        /// <summary>
        /// If true, disables built-in "skip" step action button.
        /// </summary>
        [Parameter]
        public bool ShowSkipButton { get; set; } = true;

        /// <summary>
        /// If true, disables built-in "completed"/"skipped" step result indictors shown in the actions panel.
        /// </summary>
        [Parameter]
        public bool ShowStepResultIndicator { get; set; } = true;

        /// <summary>
        /// 
        /// </summary>
        [Parameter]
        public bool MobileView { get; set; }

        /// <summary>
        /// If true, a linear loading indicator shows under the header.
        /// </summary>
        [Parameter]
        public bool Loading { get; set; }

        /// <summary>
        /// A static content that always show with all steps.
        /// </summary>
        [Parameter]
        public RenderFragment? StaticContent { get; set; }

        /// <summary>
        /// If true, action buttons have icons instead of text to gain more space.
        /// </summary>
        [Parameter]
        public bool IconActionButtons { get; set; }

        /// <summary>
        /// The predefined Mud color for header and action buttons.
        /// </summary>
        [Parameter]
        public Color Color { get; set; } = Color.Default;

        /// <summary>
        /// The variant for header and action buttons.
        /// </summary>
        [Parameter]
        public Variant Variant { get; set; }
        
        /// <summary>
        /// Choose header badge view. Default is all.
        /// </summary>
        [Parameter]
        public HeaderBadgeView HeaderBadgeView { get; set; } = HeaderBadgeView.All;

        /// <summary>
        /// Choose header text view. Default is all.
        /// </summary>
        [Parameter]
        public Size HeaderSize { get; set; } = Size.Medium;

        /// <summary>
        /// Choose header text view. Default is all.
        /// </summary>
        [Parameter]
        public HeaderTextView HeaderTextView { get; set; } = HeaderTextView.None;

        /// <summary>
        /// Choose header alignment
        /// </summary>
        [Parameter]
        public bool Vertical { get; set; }

        /// <summary>
        /// A class for provide all local strings at once.
        /// </summary>
        [Parameter]
        public StepperLocalizedStrings LocalizedStrings { get; set; } = new();

        /// <summary>
        /// The child content where MudSteps should be inside.
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        /// <summary>
        /// Custom content to be shown between the "previous" and "next" action buttons.
        /// </summary>
        /// <remarks>
        /// If set, you must also supply a <code><MudSpacer /></code> somewhere in your render fragment
        /// to ensure that the built-in action buttons are aligned correctly.
        /// </remarks>
        [Parameter]
        public RenderFragment? ActionContent { get; set; }

        /// <summary>
        /// Fires when active step changed.
        /// </summary>
        [Parameter]
        public EventCallback<int> ActiveStepChanged { get; set; }

        /// <summary>
        /// Runs a task to prevent step change. Has change direction (backwards or forwards) and target index and returns a bool value.
        /// </summary>
        [Parameter]
        public Func<StepChangeDirection, int, Task<bool>>? PreventStepChangeAsync { get; set; }

        List<MudStepExtended> _steps = new();
        List<MudStepExtended> _allSteps = new();
        /// <summary>
        /// 
        /// </summary>
        public List<MudStepExtended> Steps
        {
            get => _steps;
            protected set
            {
                if (_steps.Equals(value))
                {
                    return;
                }
                if (_steps.Select(x => x.GetHashCode()).Contains(value.GetHashCode()))
                {
                    return;
                }
                _steps = value;
            }
        }

        internal void AddStep(MudStepExtended step)
        {
            _allSteps.Add(step);
            if (!step.IsResultStep)
            {
                Steps.Add(step);
                ReorderSteps();
            }

            UpdateProgressValue();
            StateHasChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        public void ReorderSteps()
        {
            Steps = Steps.OrderBy(x => x.Order).ToList();
        }

        internal void RemoveStep(MudStepExtended step)
        {
            Steps.Remove(step);
            _allSteps.Remove(step);
            UpdateProgressValue();
            StateHasChanged();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="step"></param>
        /// <param name="skipPreventProcess"></param>
        /// <returns></returns>
        protected internal async Task SetActiveIndex(MudStepExtended step, bool skipPreventProcess = false)
        {
            await SetActiveStepByIndex(Steps.IndexOf(step), skipPreventProcess: skipPreventProcess);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="count"></param>
        /// <param name="firstCompleted"></param>
        /// <param name="skipPreventProcess"></param>
        /// <returns></returns>
        public async Task SetActiveIndex(int count, bool firstCompleted = false, bool skipPreventProcess = false)
        {
            var stepChangeDirection = (
                count == 0 ? StepChangeDirection.None :
                    count >= 1 ? StepChangeDirection.Forward :
                        StepChangeDirection.Backward
            );

            if (skipPreventProcess == false && PreventStepChangeAsync != null)
            {
                var result = await PreventStepChangeAsync.Invoke(stepChangeDirection, ActiveIndex + count);
                if (result == true)
                {
                    return;
                }
            }


            int backupActiveIndex = ActiveIndex;
            if (_animate != null && Animation == true)
            {
                await _animate.Refresh();
            }

            if (ActiveIndex == Steps.Count - 1 && !HasResultStep() && 0 < count)
            {
                return;
            }
            else if (firstCompleted)
            {
                if (HasResultStep())
                {
                    ActiveIndex = Steps.Count;
                }
            }
            else if (ActiveIndex + count < 0)
            {
                ActiveIndex = 0;
            }
            else if (ActiveIndex == Steps.Count - 1 && !IsAllStepsCompleted() && 0 < count)
            {
                ActiveIndex = Steps.IndexOf(Steps.FirstOrDefault(x => x.Status == StepStatus.Continued));
            }
            else
            {
                ActiveIndex += count;
            }

            if (backupActiveIndex != ActiveIndex)
            {
                await ActiveStepChanged.InvokeAsync(ActiveIndex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="firstCompleted"></param>
        /// <param name="skipPreventProcess"></param>
        /// <returns></returns>
        public async Task SetActiveStepByIndex(int index, bool firstCompleted = false, bool skipPreventProcess = false)
        {
            var stepChangeDirection = (
                index == ActiveIndex ? StepChangeDirection.None :
                    index > ActiveIndex ? StepChangeDirection.Forward :
                        StepChangeDirection.Backward
            );

            if (skipPreventProcess == false && PreventStepChangeAsync != null)
            {
                var result = await PreventStepChangeAsync.Invoke(stepChangeDirection, index);
                if (result == true)
                {
                    return;
                }
            }

            if (ActiveIndex == index || index < 0 || Steps.Count < index)
            {
                return;
            }

            if (Steps.Count == index && IsAllStepsCompleted() == false)
            {
                return;
            }

            if (_animate != null && Animation == true)
            {
                await _animate.Refresh();
            }

            ActiveIndex = index;
            await ActiveStepChanged.InvokeAsync(ActiveIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="moveToNextStep"></param>
        /// <returns></returns>
        public async Task CompleteStep(int index, bool moveToNextStep = true)
        {
            var isActiveStep = (index == ActiveIndex);
            if (isActiveStep)
            {
                var stepChangeDirection = (moveToNextStep ? StepChangeDirection.Forward : StepChangeDirection.None);
                if (PreventStepChangeAsync != null)
                {
                    var result = await PreventStepChangeAsync.Invoke(stepChangeDirection, index + 1);
                    if (result == true)
                    {
                        return;
                    }
                }
            }

            Steps[index].SetStatus(StepStatus.Completed);
            if (IsAllStepsCompleted())
            {
                await SetActiveIndex(1, true, true);
            }
            else if (isActiveStep && moveToNextStep)
            {
                await SetActiveIndex(1, skipPreventProcess: true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="moveToNextStep"></param>
        /// <returns></returns>
        public async Task SkipStep(int index, bool moveToNextStep = true)
        {
            var isActiveStep = (index == ActiveIndex);
            if (isActiveStep)
            {
                var stepChangeDirection = (moveToNextStep ? StepChangeDirection.Forward : StepChangeDirection.None);
                if (PreventStepChangeAsync != null)
                {
                    var result = await PreventStepChangeAsync.Invoke(stepChangeDirection, index + 1);
                    if (result == true)
                    {
                        return;
                    }
                }
            }

            Steps[index].SetStatus(StepStatus.Skipped);
            if (isActiveStep && moveToNextStep)
            {
                await SetActiveIndex(1, skipPreventProcess: true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        protected bool IsStepActive(MudStepExtended step)
        {
            return Steps.IndexOf(step) == ActiveIndex;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected int CompletedStepCount()
        {
            return Steps.Count(x => x.Status != StepStatus.Continued);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string GetNextButtonString()
        {
            return ActiveIndex >= Steps.Count - 1 ? LocalizedStrings.Finish : LocalizedStrings.Next;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected internal bool ShowResultStep()
        {
            if (IsAllStepsCompleted() && ActiveIndex == Steps.Count)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected internal bool HasResultStep()
        {
            return _allSteps.Any(x => x.IsResultStep);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool IsAllStepsCompleted()
        {
            return !Steps.Any(x => x.Status == StepStatus.Continued);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetActiveIndex()
        {
            return ActiveIndex;
        }

        /// <summary>
        /// 
        /// </summary>
        public void Reset()
        {
            Steps.ForEach(x => x.SetStatus(StepStatus.Continued));
            ActiveIndex = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="status"></param>
        public void SetStepStatus(int index, StepStatus status)
        {
            Steps[index].SetStatus(status);
        }

        /// <summary>
        /// Update all component and render again.
        /// </summary>
        public void ForceRender()
        {
            UpdateProgressValue();
            StateHasChanged();
        }

    }
}

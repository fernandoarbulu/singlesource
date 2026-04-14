// Property overview — inspector navigation helpers
window.propertyOverview = (function () {

  // Matches the `subcardNavHit` animation duration in property-shell-pages.css (2 s).
  // Extra 50 ms ensures the class is removed after the animation completes, not mid-frame.
  var SUBCARD_RING_FADE_MS = 2050;

  return {

    /**
     * Scroll the detail pane to center the target field, then apply a persistent
     * `field-highlight` on the `.pd-data-field` node.
     *
     * Highlight is PERSISTENT (not auto-timed) because it represents the user's active
     * selection — it should stay visible until the user picks a different result or clears
     * the search. The subcard ring fades after SUBCARD_RING_FADE_MS because it is only
     * contextual feedback and should not linger.
     *
     * Called from OnAfterRenderAsync after Blazor re-render completes, so the target
     * element is guaranteed to exist in the DOM before getElementById is called.
     */
    scrollAndHighlightField: function (fieldId) {
      var el = document.getElementById(fieldId);
      if (!el) return;

      // Scoped to .pd-page so rapid tab switches or other pages with the same class
      // names cannot be accidentally affected.
      document.querySelectorAll('.pd-page .field-highlight').forEach(function (prev) {
        prev.classList.remove('field-highlight');
      });
      document.querySelectorAll('.pd-page .pd-mdm-subcard--nav-hit').forEach(function (sub) {
        sub.classList.remove('pd-mdm-subcard--nav-hit');
      });

      // Remove then re-add to force a reflow. Without this, the browser skips
      // re-running the CSS animation when the same field is clicked a second time
      // (the class is already present so no transition fires).
      el.classList.remove('field-highlight');
      void el.offsetWidth; // reflow: makes the browser treat the next add as a fresh start
      el.classList.add('field-highlight');

      var sub = el.closest('.pd-mdm-subcard');
      if (sub) {
        sub.classList.remove('pd-mdm-subcard--nav-hit');
        void sub.offsetWidth;
        sub.classList.add('pd-mdm-subcard--nav-hit');
        // Subcard ring fades out after the animation ends; field highlight stays.
        setTimeout(function () { sub.classList.remove('pd-mdm-subcard--nav-hit'); }, SUBCARD_RING_FADE_MS);
      }

      // Scroll the detail pane (not the outer shell) to center the highlighted field.
      var pane = document.querySelector('.pd-page .pd-split-pane-detail.k-pane');
      if (pane && pane.contains(el)) {
        var paneRect = pane.getBoundingClientRect();
        var elRect   = el.getBoundingClientRect();
        var elTop    = elRect.top - paneRect.top + pane.scrollTop;
        var center   = elTop - pane.clientHeight / 2 + elRect.height / 2;
        pane.scrollTo({ top: Math.max(0, center), behavior: 'smooth' });
      } else {
        el.scrollIntoView({ behavior: 'smooth', block: 'center' });
      }
    },

    /** Remove any active field highlight. Called when search text changes or is cleared. */
    clearFieldHighlight: function () {
      document.querySelectorAll('.pd-page .field-highlight').forEach(function (el) {
        el.classList.remove('field-highlight');
      });
      document.querySelectorAll('.pd-page .pd-mdm-subcard--nav-hit').forEach(function (el) {
        el.classList.remove('pd-mdm-subcard--nav-hit');
      });
    },

    /** Reset the detail pane scroll to top. Called when the user switches sections. */
    scrollDetailToTop: function () {
      var pane = document.querySelector('.pd-page .pd-split-pane-detail.k-pane');
      if (pane) pane.scrollTop = 0;
    }

  };

}());

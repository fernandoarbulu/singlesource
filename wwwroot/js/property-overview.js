// Property overview — inspector navigation helpers
window.propertyOverview = {

  scrollTo: function (elementId) {
    const el = document.getElementById(elementId);
    if (!el) return;
    const root = document.querySelector('.property-shell-main');
    if (root && root.contains(el)) {
      const rootRect = root.getBoundingClientRect();
      const elRect   = el.getBoundingClientRect();
      const top = elRect.top - rootRect.top + root.scrollTop - 80;
      root.scrollTo({ top: Math.max(0, top), behavior: 'smooth' });
    } else {
      el.scrollIntoView({ behavior: 'smooth', block: 'start' });
    }
  },

  scrollToField: function (fieldId, dotNetRef) {
    const el = document.getElementById(fieldId);
    if (!el) return;
    const root = document.querySelector('.property-shell-main');
    if (root && root.contains(el)) {
      const rootRect = root.getBoundingClientRect();
      const elRect   = el.getBoundingClientRect();
      const top = elRect.top - rootRect.top + root.scrollTop - 140;
      root.scrollTo({ top: Math.max(0, top), behavior: 'smooth' });
    } else {
      el.scrollIntoView({ behavior: 'smooth', block: 'center' });
    }
    if (dotNetRef) {
      setTimeout(function () {
        dotNetRef.invokeMethodAsync('ClearFlashField');
      }, 1300);
    }
  },

  scrollToInspectorTop: function () {
    const el   = document.getElementById('pd-inspector-right');
    const root = document.querySelector('.property-shell-main');
    if (!el || !root) return;
    const rootRect = root.getBoundingClientRect();
    const elRect   = el.getBoundingClientRect();
    const top = elRect.top - rootRect.top + root.scrollTop - 16;
    root.scrollTo({ top: Math.max(0, top), behavior: 'smooth' });
  }

};

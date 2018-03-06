﻿/*
 Highcharts JS v6.0.6 (2018-02-05)

 (c) 2009-2016 Torstein Honsi

 License: www.highcharts.com/license
*/
(function (x) { "object" === typeof module && module.exports ? module.exports = x : x(Highcharts) })(function (x) {
    (function (a) {
        var r = a.deg2rad, u = a.isNumber, w = a.pick, p = a.relativeLength; a.CenteredSeriesMixin = {
            getCenter: function () {
                var a = this.options, f = this.chart, h = 2 * (a.slicedOffset || 0), c = f.plotWidth - 2 * h, f = f.plotHeight - 2 * h, b = a.center, b = [w(b[0], "50%"), w(b[1], "50%"), a.size || "100%", a.innerSize || 0], l = Math.min(c, f), g, d; for (g = 0; 4 > g; ++g)d = b[g], a = 2 > g || 2 === g && /%$/.test(d), b[g] = p(d, [c, f, l, b[2]][g]) + (a ? h : 0); b[3] > b[2] && (b[3] = b[2]);
                return b
            }, getStartAndEndRadians: function (a, f) { a = u(a) ? a : 0; f = u(f) && f > a && 360 > f - a ? f : a + 360; return { start: r * (a + -90), end: r * (f + -90) } }
        }
    })(x); (function (a) {
        function r(a, c) { this.init(a, c) } var u = a.CenteredSeriesMixin, w = a.each, p = a.extend, m = a.merge, f = a.splat; p(r.prototype, {
            coll: "pane", init: function (a, c) { this.chart = c; this.background = []; c.pane.push(this); this.setOptions(a) }, setOptions: function (a) { this.options = m(this.defaultOptions, this.chart.angular ? { background: {} } : void 0, a) }, render: function () {
                var a = this.options, c =
                    this.options.background, b = this.chart.renderer; this.group || (this.group = b.g("pane-group").attr({ zIndex: a.zIndex || 0 }).add()); this.updateCenter(); if (c) for (c = f(c), a = Math.max(c.length, this.background.length || 0), b = 0; b < a; b++)c[b] && this.axis ? this.renderBackground(m(this.defaultBackgroundOptions, c[b]), b) : this.background[b] && (this.background[b] = this.background[b].destroy(), this.background.splice(b, 1))
            }, renderBackground: function (a, c) {
                var b = "animate"; this.background[c] || (this.background[c] = this.chart.renderer.path().add(this.group),
                    b = "attr"); this.background[c][b]({ d: this.axis.getPlotBandPath(a.from, a.to, a) }).attr({ fill: a.backgroundColor, stroke: a.borderColor, "stroke-width": a.borderWidth, "class": "highcharts-pane " + (a.className || "") })
            }, defaultOptions: { center: ["50%", "50%"], size: "85%", startAngle: 0 }, defaultBackgroundOptions: { shape: "circle", borderWidth: 1, borderColor: "#cccccc", backgroundColor: { linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 }, stops: [[0, "#ffffff"], [1, "#e6e6e6"]] }, from: -Number.MAX_VALUE, innerRadius: 0, to: Number.MAX_VALUE, outerRadius: "105%" },
            updateCenter: function (a) { this.center = (a || this.axis || {}).center = u.getCenter.call(this) }, update: function (a, c) { m(!0, this.options, a); this.setOptions(this.options); this.render(); w(this.chart.axes, function (b) { b.pane === this && (b.pane = null, b.update({}, c)) }, this) }
        }); a.Pane = r
    })(x); (function (a) {
        var r = a.each, u = a.extend, w = a.map, p = a.merge, m = a.noop, f = a.pick, h = a.pInt, c = a.wrap, b, l, g = a.Axis.prototype; a = a.Tick.prototype; b = {
            getOffset: m, redraw: function () { this.isDirty = !1 }, render: function () { this.isDirty = !1 }, setScale: m, setCategories: m,
            setTitle: m
        }; l = {
            defaultRadialGaugeOptions: { labels: { align: "center", x: 0, y: null }, minorGridLineWidth: 0, minorTickInterval: "auto", minorTickLength: 10, minorTickPosition: "inside", minorTickWidth: 1, tickLength: 10, tickPosition: "inside", tickWidth: 2, title: { rotation: 0 }, zIndex: 2 }, defaultRadialXOptions: { gridLineWidth: 1, labels: { align: null, distance: 15, x: 0, y: null, style: { textOverflow: "none" } }, maxPadding: 0, minPadding: 0, showLastLabel: !1, tickLength: 0 }, defaultRadialYOptions: {
                gridLineInterpolation: "circle", labels: {
                    align: "right",
                    x: -3, y: -2
                }, showLastLabel: !1, title: { x: 4, text: null, rotation: 90 }
            }, setOptions: function (b) { b = this.options = p(this.defaultOptions, this.defaultRadialOptions, b); b.plotBands || (b.plotBands = []) }, getOffset: function () { g.getOffset.call(this); this.chart.axisOffset[this.side] = 0 }, getLinePath: function (b, c) {
                b = this.center; var d = this.chart, e = f(c, b[2] / 2 - this.offset); this.isCircular || void 0 !== c ? (c = this.chart.renderer.symbols.arc(this.left + b[0], this.top + b[1], e, e, { start: this.startAngleRad, end: this.endAngleRad, open: !0, innerR: 0 }),
                    c.xBounds = [this.left + b[0]], c.yBounds = [this.top + b[1] - e]) : (c = this.postTranslate(this.angleRad, e), c = ["M", b[0] + d.plotLeft, b[1] + d.plotTop, "L", c.x, c.y]); return c
            }, setAxisTranslation: function () { g.setAxisTranslation.call(this); this.center && (this.transA = this.isCircular ? (this.endAngleRad - this.startAngleRad) / (this.max - this.min || 1) : this.center[2] / 2 / (this.max - this.min || 1), this.minPixelPadding = this.isXAxis ? this.transA * this.minPointOffset : 0) }, beforeSetTickPositions: function () {
                if (this.autoConnect = this.isCircular &&
                    void 0 === f(this.userMax, this.options.max) && this.endAngleRad - this.startAngleRad === 2 * Math.PI) this.max += this.categories && 1 || this.pointRange || this.closestPointRange || 0
            }, setAxisSize: function () { g.setAxisSize.call(this); this.isRadial && (this.pane.updateCenter(this), this.isCircular && (this.sector = this.endAngleRad - this.startAngleRad), this.len = this.width = this.height = this.center[2] * f(this.sector, 1) / 2) }, getPosition: function (b, c) {
                return this.postTranslate(this.isCircular ? this.translate(b) : this.angleRad, f(this.isCircular ?
                    c : this.translate(b), this.center[2] / 2) - this.offset)
            }, postTranslate: function (b, c) { var d = this.chart, e = this.center; b = this.startAngleRad + b; return { x: d.plotLeft + e[0] + Math.cos(b) * c, y: d.plotTop + e[1] + Math.sin(b) * c } }, getPlotBandPath: function (b, c, a) {
                var d = this.center, e = this.startAngleRad, l = d[2] / 2, k = [f(a.outerRadius, "100%"), a.innerRadius, f(a.thickness, 10)], g = Math.min(this.offset, 0), t = /%$/, m, p = this.isCircular; "polygon" === this.options.gridLineInterpolation ? d = this.getPlotLinePath(b).concat(this.getPlotLinePath(c,
                    !0)) : (b = Math.max(b, this.min), c = Math.min(c, this.max), p || (k[0] = this.translate(b), k[1] = this.translate(c)), k = w(k, function (b) { t.test(b) && (b = h(b, 10) * l / 100); return b }), "circle" !== a.shape && p ? (b = e + this.translate(b), c = e + this.translate(c)) : (b = -Math.PI / 2, c = 1.5 * Math.PI, m = !0), k[0] -= g, k[2] -= g, d = this.chart.renderer.symbols.arc(this.left + d[0], this.top + d[1], k[0], k[0], { start: Math.min(b, c), end: Math.max(b, c), innerR: f(k[1], k[0] - k[2]), open: m })); return d
            }, getPlotLinePath: function (b, c) {
                var d = this, e = d.center, a = d.chart, l = d.getPosition(b),
                g, f, t; d.isCircular ? t = ["M", e[0] + a.plotLeft, e[1] + a.plotTop, "L", l.x, l.y] : "circle" === d.options.gridLineInterpolation ? (b = d.translate(b)) && (t = d.getLinePath(0, b)) : (r(a.xAxis, function (b) { b.pane === d.pane && (g = b) }), t = [], b = d.translate(b), e = g.tickPositions, g.autoConnect && (e = e.concat([e[0]])), c && (e = [].concat(e).reverse()), r(e, function (c, d) { f = g.getPosition(c, b); t.push(d ? "L" : "M", f.x, f.y) })); return t
            }, getTitlePosition: function () {
                var b = this.center, c = this.chart, a = this.options.title; return {
                    x: c.plotLeft + b[0] + (a.x || 0),
                    y: c.plotTop + b[1] - { high: .5, middle: .25, low: 0 }[a.align] * b[2] + (a.y || 0)
                }
            }
        }; c(g, "init", function (c, e, a) {
            var d = e.angular, k = e.polar, g = a.isX, y = d && g, m, t = e.options, h = a.pane || 0, r = this.pane = e.pane && e.pane[h], h = r && r.options; if (d) { if (u(this, y ? b : l), m = !g) this.defaultRadialOptions = this.defaultRadialGaugeOptions } else k && (u(this, l), this.defaultRadialOptions = (m = g) ? this.defaultRadialXOptions : p(this.defaultYAxisOptions, this.defaultRadialYOptions)); d || k ? (this.isRadial = !0, e.inverted = !1, t.chart.zoomType = null) : this.isRadial =
                !1; r && m && (r.axis = this); c.call(this, e, a); !y && r && (d || k) && (c = this.options, this.angleRad = (c.angle || 0) * Math.PI / 180, this.startAngleRad = (h.startAngle - 90) * Math.PI / 180, this.endAngleRad = (f(h.endAngle, h.startAngle + 360) - 90) * Math.PI / 180, this.offset = c.offset || 0, this.isCircular = m)
        }); c(g, "autoLabelAlign", function (b) { if (!this.isRadial) return b.apply(this, [].slice.call(arguments, 1)) }); c(a, "getPosition", function (b, c, a, l, g) { var d = this.axis; return d.getPosition ? d.getPosition(a) : b.call(this, c, a, l, g) }); c(a, "getLabelPosition",
            function (b, c, a, l, g, q, y, m, t) {
                var d = this.axis, e = q.y, k = 20, n = q.align, v = (d.translate(this.pos) + d.startAngleRad + Math.PI / 2) / Math.PI * 180 % 360; d.isRadial ? (b = d.getPosition(this.pos, d.center[2] / 2 + f(q.distance, -25)), "auto" === q.rotation ? l.attr({ rotation: v }) : null === e && (e = d.chart.renderer.fontMetrics(l.styles.fontSize).b - l.getBBox().height / 2), null === n && (d.isCircular ? (this.label.getBBox().width > d.len * d.tickInterval / (d.max - d.min) && (k = 0), n = v > k && v < 180 - k ? "left" : v > 180 + k && v < 360 - k ? "right" : "center") : n = "center", l.attr({ align: n })),
                    b.x += q.x, b.y += e) : b = b.call(this, c, a, l, g, q, y, m, t); return b
            }); c(a, "getMarkPath", function (b, c, a, l, g, q, y) { var d = this.axis; d.isRadial ? (b = d.getPosition(this.pos, d.center[2] / 2 + l), c = ["M", c, a, "L", b.x, b.y]) : c = b.call(this, c, a, l, g, q, y); return c })
    })(x); (function (a) {
        var r = a.each, u = a.pick, w = a.defined, p = a.seriesType, m = a.seriesTypes, f = a.Series.prototype, h = a.Point.prototype; p("arearange", "area", {
            lineWidth: 1, threshold: null, tooltip: { pointFormat: '\x3cspan style\x3d"color:{series.color}"\x3e\u25cf\x3c/span\x3e {series.name}: \x3cb\x3e{point.low}\x3c/b\x3e - \x3cb\x3e{point.high}\x3c/b\x3e\x3cbr/\x3e' },
            trackByArea: !0, dataLabels: { align: null, verticalAlign: null, xLow: 0, xHigh: 0, yLow: 0, yHigh: 0 }
        }, {
            pointArrayMap: ["low", "high"], dataLabelCollections: ["dataLabel", "dataLabelUpper"], toYData: function (c) { return [c.low, c.high] }, pointValKey: "low", deferTranslatePolar: !0, highToXY: function (c) { var b = this.chart, a = this.xAxis.postTranslate(c.rectPlotX, this.yAxis.len - c.plotHigh); c.plotHighX = a.x - b.plotLeft; c.plotHigh = a.y - b.plotTop; c.plotLowX = c.plotX }, translate: function () {
                var c = this, b = c.yAxis, a = !!c.modifyValue; m.area.prototype.translate.apply(c);
                r(c.points, function (l) { var d = l.low, e = l.high, k = l.plotY; null === e || null === d ? (l.isNull = !0, l.plotY = null) : (l.plotLow = k, l.plotHigh = b.translate(a ? c.modifyValue(e, l) : e, 0, 1, 0, 1), a && (l.yBottom = l.plotHigh)) }); this.chart.polar && r(this.points, function (b) { c.highToXY(b); b.tooltipPos = [(b.plotHighX + b.plotLowX) / 2, (b.plotHigh + b.plotLow) / 2] })
            }, getGraphPath: function (c) {
                var b = [], a = [], g, d = m.area.prototype.getGraphPath, e, k, v; v = this.options; var n = this.chart.polar && !1 !== v.connectEnds, q = v.connectNulls, y = v.step; c = c || this.points;
                for (g = c.length; g--;)e = c[g], e.isNull || n || q || c[g + 1] && !c[g + 1].isNull || a.push({ plotX: e.plotX, plotY: e.plotY, doCurve: !1 }), k = { polarPlotY: e.polarPlotY, rectPlotX: e.rectPlotX, yBottom: e.yBottom, plotX: u(e.plotHighX, e.plotX), plotY: e.plotHigh, isNull: e.isNull }, a.push(k), b.push(k), e.isNull || n || q || c[g - 1] && !c[g - 1].isNull || a.push({ plotX: e.plotX, plotY: e.plotY, doCurve: !1 }); c = d.call(this, c); y && (!0 === y && (y = "left"), v.step = { left: "right", center: "center", right: "left" }[y]); b = d.call(this, b); a = d.call(this, a); v.step = y; v = [].concat(c,
                    b); this.chart.polar || "M" !== a[0] || (a[0] = "L"); this.graphPath = v; this.areaPath = c.concat(a); v.isArea = !0; v.xMap = c.xMap; this.areaPath.xMap = c.xMap; return v
            }, drawDataLabels: function () {
                var c = this.data, b = c.length, a, g = [], d = this.options.dataLabels, e = d.align, k = d.verticalAlign, v = d.inside, n, q, y = this.chart.inverted; if (d.enabled || this._hasPointLabels) {
                    for (a = b; a--;)if (n = c[a]) q = v ? n.plotHigh < n.plotLow : n.plotHigh > n.plotLow, n.y = n.high, n._plotY = n.plotY, n.plotY = n.plotHigh, g[a] = n.dataLabel, n.dataLabel = n.dataLabelUpper, n.below =
                        q, y ? e || (d.align = q ? "right" : "left") : k || (d.verticalAlign = q ? "top" : "bottom"), d.x = d.xHigh, d.y = d.yHigh; f.drawDataLabels && f.drawDataLabels.apply(this, arguments); for (a = b; a--;)if (n = c[a]) q = v ? n.plotHigh < n.plotLow : n.plotHigh > n.plotLow, n.dataLabelUpper = n.dataLabel, n.dataLabel = g[a], n.y = n.low, n.plotY = n._plotY, n.below = !q, y ? e || (d.align = q ? "left" : "right") : k || (d.verticalAlign = q ? "bottom" : "top"), d.x = d.xLow, d.y = d.yLow; f.drawDataLabels && f.drawDataLabels.apply(this, arguments)
                } d.align = e; d.verticalAlign = k
            }, alignDataLabel: function () {
                m.column.prototype.alignDataLabel.apply(this,
                    arguments)
            }, drawPoints: function () {
                var c = this.points.length, b, a; f.drawPoints.apply(this, arguments); for (a = 0; a < c;)b = this.points[a], b.lowerGraphic = b.graphic, b.graphic = b.upperGraphic, b._plotY = b.plotY, b._plotX = b.plotX, b.plotY = b.plotHigh, w(b.plotHighX) && (b.plotX = b.plotHighX), b._isInside = b.isInside, this.chart.polar || (b.isInside = b.isTopInside = void 0 !== b.plotY && 0 <= b.plotY && b.plotY <= this.yAxis.len && 0 <= b.plotX && b.plotX <= this.xAxis.len), a++; f.drawPoints.apply(this, arguments); for (a = 0; a < c;)b = this.points[a], b.upperGraphic =
                    b.graphic, b.graphic = b.lowerGraphic, b.isInside = b._isInside, b.plotY = b._plotY, b.plotX = b._plotX, a++
            }, setStackedPoints: a.noop
            }, {
                setState: function () {
                    var c = this.state, b = this.series, a = b.chart.polar; w(this.plotHigh) || (this.plotHigh = b.yAxis.toPixels(this.high, !0)); w(this.plotLow) || (this.plotLow = this.plotY = b.yAxis.toPixels(this.low, !0)); b.stateMarkerGraphic && (b.lowerStateMarkerGraphic = b.stateMarkerGraphic, b.stateMarkerGraphic = b.upperStateMarkerGraphic); this.graphic = this.upperGraphic; this.plotY = this.plotHigh;
                    a && (this.plotX = this.plotHighX); h.setState.apply(this, arguments); this.state = c; this.plotY = this.plotLow; this.graphic = this.lowerGraphic; a && (this.plotX = this.plotLowX); b.stateMarkerGraphic && (b.upperStateMarkerGraphic = b.stateMarkerGraphic, b.stateMarkerGraphic = b.lowerStateMarkerGraphic, b.lowerStateMarkerGraphic = void 0); h.setState.apply(this, arguments)
                }, haloPath: function () {
                    var c = this.series.chart.polar, b = []; this.plotY = this.plotLow; c && (this.plotX = this.plotLowX); this.isInside && (b = h.haloPath.apply(this, arguments));
                    this.plotY = this.plotHigh; c && (this.plotX = this.plotHighX); this.isTopInside && (b = b.concat(h.haloPath.apply(this, arguments))); return b
                }, destroyElements: function () { r(["lowerGraphic", "upperGraphic"], function (c) { this[c] && (this[c] = this[c].destroy()) }, this); this.graphic = null; return h.destroyElements.apply(this, arguments) }
            })
    })(x); (function (a) { var r = a.seriesType; r("areasplinerange", "arearange", null, { getPointSpline: a.seriesTypes.spline.prototype.getPointSpline }) })(x); (function (a) {
        var r = a.defaultPlotOptions, u =
            a.each, w = a.merge, p = a.noop, m = a.pick, f = a.seriesType, h = a.seriesTypes.column.prototype; f("columnrange", "arearange", w(r.column, r.arearange, { pointRange: null, marker: null, states: { hover: { halo: !1 } } }), {
                translate: function () {
                    var c = this, b = c.yAxis, a = c.xAxis, g = a.startAngleRad, d, e = c.chart, k = c.xAxis.isRadial, v = Math.max(e.chartWidth, e.chartHeight) + 999, n; h.translate.apply(c); u(c.points, function (l) {
                        var q = l.shapeArgs, f = c.options.minPointLength, t, h; l.plotHigh = n = Math.min(Math.max(-v, b.translate(l.high, 0, 1, 0, 1)), v); l.plotLow =
                            Math.min(Math.max(-v, l.plotY), v); h = n; t = m(l.rectPlotY, l.plotY) - n; Math.abs(t) < f ? (f -= t, t += f, h -= f / 2) : 0 > t && (t *= -1, h -= t); k ? (d = l.barX + g, l.shapeType = "path", l.shapeArgs = { d: c.polarArc(h + t, h, d, d + l.pointWidth) }) : (q.height = t, q.y = h, l.tooltipPos = e.inverted ? [b.len + b.pos - e.plotLeft - h - t / 2, a.len + a.pos - e.plotTop - q.x - q.width / 2, t] : [a.left - e.plotLeft + q.x + q.width / 2, b.pos - e.plotTop + h + t / 2, t])
                    })
                }, directTouch: !0, trackerGroups: ["group", "dataLabelsGroup"], drawGraph: p, getSymbol: p, crispCol: h.crispCol, drawPoints: h.drawPoints, drawTracker: h.drawTracker,
                getColumnMetrics: h.getColumnMetrics, pointAttribs: h.pointAttribs, animate: function () { return h.animate.apply(this, arguments) }, polarArc: function () { return h.polarArc.apply(this, arguments) }, translate3dPoints: function () { return h.translate3dPoints.apply(this, arguments) }, translate3dShapes: function () { return h.translate3dShapes.apply(this, arguments) }
            }, { setState: h.pointClass.prototype.setState })
    })(x); (function (a) {
        var r = a.each, u = a.isNumber, w = a.merge, p = a.pick, m = a.pInt, f = a.Series, h = a.seriesType, c = a.TrackerMixin;
        h("gauge", "line", { dataLabels: { enabled: !0, defer: !1, y: 15, borderRadius: 3, crop: !1, verticalAlign: "top", zIndex: 2, borderWidth: 1, borderColor: "#cccccc" }, dial: {}, pivot: {}, tooltip: { headerFormat: "" }, showInLegend: !1 }, {
            angular: !0, directTouch: !0, drawGraph: a.noop, fixedBox: !0, forceDL: !0, noSharedTooltip: !0, trackerGroups: ["group", "dataLabelsGroup"], translate: function () {
                var b = this.yAxis, c = this.options, a = b.center; this.generatePoints(); r(this.points, function (d) {
                    var e = w(c.dial, d.dial), l = m(p(e.radius, 80)) * a[2] / 200, g = m(p(e.baseLength,
                        70)) * l / 100, n = m(p(e.rearLength, 10)) * l / 100, q = e.baseWidth || 3, f = e.topWidth || 1, h = c.overshoot, t = b.startAngleRad + b.translate(d.y, null, null, null, !0); u(h) ? (h = h / 180 * Math.PI, t = Math.max(b.startAngleRad - h, Math.min(b.endAngleRad + h, t))) : !1 === c.wrap && (t = Math.max(b.startAngleRad, Math.min(b.endAngleRad, t))); t = 180 * t / Math.PI; d.shapeType = "path"; d.shapeArgs = { d: e.path || ["M", -n, -q / 2, "L", g, -q / 2, l, -f / 2, l, f / 2, g, q / 2, -n, q / 2, "z"], translateX: a[0], translateY: a[1], rotation: t }; d.plotX = a[0]; d.plotY = a[1]
                })
            }, drawPoints: function () {
                var b =
                    this, c = b.yAxis.center, a = b.pivot, d = b.options, e = d.pivot, k = b.chart.renderer; r(b.points, function (c) { var a = c.graphic, e = c.shapeArgs, l = e.d, g = w(d.dial, c.dial); a ? (a.animate(e), e.d = l) : (c.graphic = k[c.shapeType](e).attr({ rotation: e.rotation, zIndex: 1 }).addClass("highcharts-dial").add(b.group), c.graphic.attr({ stroke: g.borderColor || "none", "stroke-width": g.borderWidth || 0, fill: g.backgroundColor || "#000000" })) }); a ? a.animate({ translateX: c[0], translateY: c[1] }) : (b.pivot = k.circle(0, 0, p(e.radius, 5)).attr({ zIndex: 2 }).addClass("highcharts-pivot").translate(c[0],
                        c[1]).add(b.group), b.pivot.attr({ "stroke-width": e.borderWidth || 0, stroke: e.borderColor || "#cccccc", fill: e.backgroundColor || "#000000" }))
            }, animate: function (b) { var c = this; b || (r(c.points, function (b) { var a = b.graphic; a && (a.attr({ rotation: 180 * c.yAxis.startAngleRad / Math.PI }), a.animate({ rotation: b.shapeArgs.rotation }, c.options.animation)) }), c.animate = null) }, render: function () {
            this.group = this.plotGroup("group", "series", this.visible ? "visible" : "hidden", this.options.zIndex, this.chart.seriesGroup); f.prototype.render.call(this);
                this.group.clip(this.chart.clipRect)
            }, setData: function (b, c) { f.prototype.setData.call(this, b, !1); this.processData(); this.generatePoints(); p(c, !0) && this.chart.redraw() }, drawTracker: c && c.drawTrackerPoint
        }, { setState: function (b) { this.state = b } })
    })(x); (function (a) {
        var r = a.each, u = a.noop, w = a.pick, p = a.seriesType, m = a.seriesTypes; p("boxplot", "column", {
            threshold: null, tooltip: { pointFormat: '\x3cspan style\x3d"color:{point.color}"\x3e\u25cf\x3c/span\x3e \x3cb\x3e {series.name}\x3c/b\x3e\x3cbr/\x3eMaximum: {point.high}\x3cbr/\x3eUpper quartile: {point.q3}\x3cbr/\x3eMedian: {point.median}\x3cbr/\x3eLower quartile: {point.q1}\x3cbr/\x3eMinimum: {point.low}\x3cbr/\x3e' },
            whiskerLength: "50%", fillColor: "#ffffff", lineWidth: 1, medianWidth: 2, whiskerWidth: 2
        }, {
            pointArrayMap: ["low", "q1", "median", "q3", "high"], toYData: function (a) { return [a.low, a.q1, a.median, a.q3, a.high] }, pointValKey: "high", pointAttribs: function () { return {} }, drawDataLabels: u, translate: function () { var a = this.yAxis, h = this.pointArrayMap; m.column.prototype.translate.apply(this); r(this.points, function (c) { r(h, function (b) { null !== c[b] && (c[b + "Plot"] = a.translate(c[b], 0, 1, 0, 1)) }) }) }, drawPoints: function () {
                var a = this, h = a.options,
                c = a.chart.renderer, b, l, g, d, e, k, v = 0, n, q, m, p, t = !1 !== a.doQuartiles, u, A = a.options.whiskerLength; r(a.points, function (f) {
                    var y = f.graphic, r = y ? "animate" : "attr", K = f.shapeArgs, x = {}, C = {}, I = {}, J = {}, B = f.color || a.color; void 0 !== f.plotY && (n = K.width, q = Math.floor(K.x), m = q + n, p = Math.round(n / 2), b = Math.floor(t ? f.q1Plot : f.lowPlot), l = Math.floor(t ? f.q3Plot : f.lowPlot), g = Math.floor(f.highPlot), d = Math.floor(f.lowPlot), y || (f.graphic = y = c.g("point").add(a.group), f.stem = c.path().addClass("highcharts-boxplot-stem").add(y), A && (f.whiskers =
                        c.path().addClass("highcharts-boxplot-whisker").add(y)), t && (f.box = c.path(void 0).addClass("highcharts-boxplot-box").add(y)), f.medianShape = c.path(void 0).addClass("highcharts-boxplot-median").add(y)), C.stroke = f.stemColor || h.stemColor || B, C["stroke-width"] = w(f.stemWidth, h.stemWidth, h.lineWidth), C.dashstyle = f.stemDashStyle || h.stemDashStyle, f.stem.attr(C), A && (I.stroke = f.whiskerColor || h.whiskerColor || B, I["stroke-width"] = w(f.whiskerWidth, h.whiskerWidth, h.lineWidth), f.whiskers.attr(I)), t && (x.fill = f.fillColor ||
                            h.fillColor || B, x.stroke = h.lineColor || B, x["stroke-width"] = h.lineWidth || 0, f.box.attr(x)), J.stroke = f.medianColor || h.medianColor || B, J["stroke-width"] = w(f.medianWidth, h.medianWidth, h.lineWidth), f.medianShape.attr(J), k = f.stem.strokeWidth() % 2 / 2, v = q + p + k, f.stem[r]({ d: ["M", v, l, "L", v, g, "M", v, b, "L", v, d] }), t && (k = f.box.strokeWidth() % 2 / 2, b = Math.floor(b) + k, l = Math.floor(l) + k, q += k, m += k, f.box[r]({ d: ["M", q, l, "L", q, b, "L", m, b, "L", m, l, "L", q, l, "z"] })), A && (k = f.whiskers.strokeWidth() % 2 / 2, g += k, d += k, u = /%$/.test(A) ? p * parseFloat(A) /
                                100 : A / 2, f.whiskers[r]({ d: ["M", v - u, g, "L", v + u, g, "M", v - u, d, "L", v + u, d] })), e = Math.round(f.medianPlot), k = f.medianShape.strokeWidth() % 2 / 2, e += k, f.medianShape[r]({ d: ["M", q, e, "L", m, e] }))
                })
            }, setStackedPoints: u
            })
    })(x); (function (a) {
        var r = a.each, u = a.noop, w = a.seriesType, p = a.seriesTypes; w("errorbar", "boxplot", {
            color: "#000000", grouping: !1, linkedTo: ":previous", tooltip: { pointFormat: '\x3cspan style\x3d"color:{point.color}"\x3e\u25cf\x3c/span\x3e {series.name}: \x3cb\x3e{point.low}\x3c/b\x3e - \x3cb\x3e{point.high}\x3c/b\x3e\x3cbr/\x3e' },
            whiskerWidth: null
        }, { type: "errorbar", pointArrayMap: ["low", "high"], toYData: function (a) { return [a.low, a.high] }, pointValKey: "high", doQuartiles: !1, drawDataLabels: p.arearange ? function () { var a = this.pointValKey; p.arearange.prototype.drawDataLabels.call(this); r(this.data, function (f) { f.y = f[a] }) } : u, getColumnMetrics: function () { return this.linkedParent && this.linkedParent.columnMetrics || p.column.prototype.getColumnMetrics.call(this) } })
    })(x); (function (a) {
        var r = a.correctFloat, u = a.isNumber, w = a.pick, p = a.Point, m = a.Series,
        f = a.seriesType, h = a.seriesTypes; f("waterfall", "column", { dataLabels: { inside: !0 }, lineWidth: 1, lineColor: "#333333", dashStyle: "dot", borderColor: "#333333", states: { hover: { lineWidthPlus: 0 } } }, {
            pointValKey: "y", translate: function () {
                var c = this.options, b = this.yAxis, a, g, d, e, k, f, n, q, m, p, t = w(c.minPointLength, 5), u = t / 2, x = c.threshold, E = c.stacking, z; h.column.prototype.translate.apply(this); q = m = x; g = this.points; a = 0; for (c = g.length; a < c; a++)d = g[a], n = this.processedYData[a], e = d.shapeArgs, k = E && b.stacks[(this.negStacks && n < x ? "-" :
                    "") + this.stackKey], z = this.getStackIndicator(z, d.x, this.index), p = w(k && k[d.x].points[z.key], [0, n]), d.isSum ? d.y = r(n) : d.isIntermediateSum && (d.y = r(n - m)), f = Math.max(q, q + d.y) + p[0], e.y = b.translate(f, 0, 1, 0, 1), d.isSum ? (e.y = b.translate(p[1], 0, 1, 0, 1), e.height = Math.min(b.translate(p[0], 0, 1, 0, 1), b.len) - e.y) : d.isIntermediateSum ? (e.y = b.translate(p[1], 0, 1, 0, 1), e.height = Math.min(b.translate(m, 0, 1, 0, 1), b.len) - e.y, m = p[1]) : (e.height = 0 < n ? b.translate(q, 0, 1, 0, 1) - e.y : b.translate(q, 0, 1, 0, 1) - b.translate(q - n, 0, 1, 0, 1), q += k &&
                        k[d.x] ? k[d.x].total : n), 0 > e.height && (e.y += e.height, e.height *= -1), d.plotY = e.y = Math.round(e.y) - this.borderWidth % 2 / 2, e.height = Math.max(Math.round(e.height), .001), d.yBottom = e.y + e.height, e.height <= t && !d.isNull ? (e.height = t, e.y -= u, d.plotY = e.y, d.minPointLengthOffset = 0 > d.y ? -u : u) : d.minPointLengthOffset = 0, e = d.plotY + (d.negative ? e.height : 0), this.chart.inverted ? d.tooltipPos[0] = b.len - e : d.tooltipPos[1] = e
            }, processData: function (c) {
                var b = this.yData, a = this.options.data, g, d = b.length, e, k, f, n, q, h; k = e = f = n = this.options.threshold ||
                    0; for (h = 0; h < d; h++)q = b[h], g = a && a[h] ? a[h] : {}, "sum" === q || g.isSum ? b[h] = r(k) : "intermediateSum" === q || g.isIntermediateSum ? b[h] = r(e) : (k += q, e += q), f = Math.min(k, f), n = Math.max(k, n); m.prototype.processData.call(this, c); this.options.stacking || (this.dataMin = f, this.dataMax = n)
            }, toYData: function (c) { return c.isSum ? 0 === c.x ? null : "sum" : c.isIntermediateSum ? 0 === c.x ? null : "intermediateSum" : c.y }, pointAttribs: function (c, b) {
                var a = this.options.upColor; a && !c.options.color && (c.color = 0 < c.y ? a : null); c = h.column.prototype.pointAttribs.call(this,
                    c, b); delete c.dashstyle; return c
            }, getGraphPath: function () { return ["M", 0, 0] }, getCrispPath: function () { var c = this.data, b = c.length, a = this.graph.strokeWidth() + this.borderWidth, a = Math.round(a) % 2 / 2, g = this.xAxis.reversed, d = this.yAxis.reversed, e = [], k, f, n; for (n = 1; n < b; n++) { f = c[n].shapeArgs; k = c[n - 1].shapeArgs; f = ["M", k.x + (g ? 0 : k.width), k.y + c[n - 1].minPointLengthOffset + a, "L", f.x + (g ? k.width : 0), k.y + c[n - 1].minPointLengthOffset + a]; if (0 > c[n - 1].y && !d || 0 < c[n - 1].y && d) f[2] += k.height, f[5] += k.height; e = e.concat(f) } return e },
            drawGraph: function () { m.prototype.drawGraph.call(this); this.graph.attr({ d: this.getCrispPath() }) }, setStackedPoints: function () { var c = this.options, b, a; m.prototype.setStackedPoints.apply(this, arguments); b = this.stackedYData ? this.stackedYData.length : 0; for (a = 1; a < b; a++)c.data[a].isSum || c.data[a].isIntermediateSum || (this.stackedYData[a] += this.stackedYData[a - 1]) }, getExtremes: function () { if (this.options.stacking) return m.prototype.getExtremes.apply(this, arguments) }
        }, {
            getClassName: function () {
                var c = p.prototype.getClassName.call(this);
                this.isSum ? c += " highcharts-sum" : this.isIntermediateSum && (c += " highcharts-intermediate-sum"); return c
            }, isValid: function () { return u(this.y, !0) || this.isSum || this.isIntermediateSum }
            })
    })(x); (function (a) {
        var r = a.Series, u = a.seriesType, w = a.seriesTypes; u("polygon", "scatter", { marker: { enabled: !1, states: { hover: { enabled: !1 } } }, stickyTracking: !1, tooltip: { followPointer: !0, pointFormat: "" }, trackByArea: !0 }, {
            type: "polygon", getGraphPath: function () {
                for (var a = r.prototype.getGraphPath.call(this), m = a.length + 1; m--;)(m === a.length ||
                    "M" === a[m]) && 0 < m && a.splice(m, 0, "z"); return this.areaPath = a
            }, drawGraph: function () { this.options.fillColor = this.color; w.area.prototype.drawGraph.call(this) }, drawLegendSymbol: a.LegendSymbolMixin.drawRectangle, drawTracker: r.prototype.drawTracker, setStackedPoints: a.noop
        })
    })(x); (function (a) {
        var r = a.arrayMax, u = a.arrayMin, w = a.Axis, p = a.color, m = a.each, f = a.isNumber, h = a.noop, c = a.pick, b = a.pInt, l = a.Point, g = a.Series, d = a.seriesType, e = a.seriesTypes; d("bubble", "scatter", {
            dataLabels: {
                formatter: function () { return this.point.z },
                inside: !0, verticalAlign: "middle"
            }, marker: { lineColor: null, lineWidth: 1, fillOpacity: .5, radius: null, states: { hover: { radiusPlus: 0 } }, symbol: "circle" }, minSize: 8, maxSize: "20%", softThreshold: !1, states: { hover: { halo: { size: 5 } } }, tooltip: { pointFormat: "({point.x}, {point.y}), Size: {point.z}" }, turboThreshold: 0, zThreshold: 0, zoneAxis: "z"
        }, {
            pointArrayMap: ["y", "z"], parallelArrays: ["x", "y", "z"], trackerGroups: ["group", "dataLabelsGroup"], specialGroup: "group", bubblePadding: !0, zoneAxis: "z", directTouch: !0, pointAttribs: function (b,
                c) { var a = this.options.marker.fillOpacity; b = g.prototype.pointAttribs.call(this, b, c); 1 !== a && (b.fill = p(b.fill).setOpacity(a).get("rgba")); return b }, getRadii: function (b, c, a, d) { var e, f, k, g = this.zData, n = [], l = this.options, q = "width" !== l.sizeBy, h = l.zThreshold, m = c - b; f = 0; for (e = g.length; f < e; f++)k = g[f], l.sizeByAbsoluteValue && null !== k && (k = Math.abs(k - h), c = Math.max(c - h, Math.abs(b - h)), b = 0), null === k ? k = null : k < b ? k = a / 2 - 1 : (k = 0 < m ? (k - b) / m : .5, q && 0 <= k && (k = Math.sqrt(k)), k = Math.ceil(a + k * (d - a)) / 2), n.push(k); this.radii = n }, animate: function (b) {
                    var c =
                        this.options.animation; b || (m(this.points, function (b) { var a = b.graphic, d; a && a.width && (d = { x: a.x, y: a.y, width: a.width, height: a.height }, a.attr({ x: b.plotX, y: b.plotY, width: 1, height: 1 }), a.animate(d, c)) }), this.animate = null)
                }, translate: function () {
                    var b, c = this.data, d, g, l = this.radii; e.scatter.prototype.translate.call(this); for (b = c.length; b--;)d = c[b], g = l ? l[b] : 0, f(g) && g >= this.minPxSize / 2 ? (d.marker = a.extend(d.marker, { radius: g, width: 2 * g, height: 2 * g }), d.dlBox = { x: d.plotX - g, y: d.plotY - g, width: 2 * g, height: 2 * g }) : d.shapeArgs =
                        d.plotY = d.dlBox = void 0
                }, alignDataLabel: e.column.prototype.alignDataLabel, buildKDTree: h, applyZones: h
            }, { haloPath: function (b) { return l.prototype.haloPath.call(this, 0 === b ? 0 : (this.marker ? this.marker.radius || 0 : 0) + b) }, ttBelow: !1 }); w.prototype.beforePadding = function () {
                var a = this, d = this.len, e = this.chart, g = 0, l = d, h = this.isXAxis, p = h ? "xData" : "yData", w = this.min, x = {}, E = Math.min(e.plotWidth, e.plotHeight), z = Number.MAX_VALUE, F = -Number.MAX_VALUE, G = this.max - w, D = d / G, H = []; m(this.series, function (d) {
                    var g = d.options; !d.bubblePadding ||
                        !d.visible && e.options.chart.ignoreHiddenSeries || (a.allowZoomOutside = !0, H.push(d), h && (m(["minSize", "maxSize"], function (a) { var c = g[a], d = /%$/.test(c), c = b(c); x[a] = d ? E * c / 100 : c }), d.minPxSize = x.minSize, d.maxPxSize = Math.max(x.maxSize, x.minSize), d = d.zData, d.length && (z = c(g.zMin, Math.min(z, Math.max(u(d), !1 === g.displayNegative ? g.zThreshold : -Number.MAX_VALUE))), F = c(g.zMax, Math.max(F, r(d))))))
                }); m(H, function (b) {
                    var c = b[p], d = c.length, e; h && b.getRadii(z, F, b.minPxSize, b.maxPxSize); if (0 < G) for (; d--;)f(c[d]) && a.dataMin <=
                        c[d] && c[d] <= a.dataMax && (e = b.radii[d], g = Math.min((c[d] - w) * D - e, g), l = Math.max((c[d] - w) * D + e, l))
                }); H.length && 0 < G && !this.isLog && (l -= d, D *= (d + g - l) / d, m([["min", "userMin", g], ["max", "userMax", l]], function (b) { void 0 === c(a.options[b[0]], a[b[1]]) && (a[b[0]] += b[2] / D) }))
            }
    })(x); (function (a) {
        function r(c, b) {
            var a = this.chart, g = this.options.animation, d = this.group, e = this.markerGroup, f = this.xAxis.center, h = a.plotLeft, n = a.plotTop; a.polar ? a.renderer.isSVG && (!0 === g && (g = {}), b ? (c = {
                translateX: f[0] + h, translateY: f[1] + n, scaleX: .001,
                scaleY: .001
            }, d.attr(c), e && e.attr(c)) : (c = { translateX: h, translateY: n, scaleX: 1, scaleY: 1 }, d.animate(c, g), e && e.animate(c, g), this.animate = null)) : c.call(this, b)
        } var u = a.each, w = a.pick, p = a.seriesTypes, m = a.wrap, f = a.Series.prototype, h = a.Pointer.prototype; f.searchPointByAngle = function (a) { var b = this.chart, c = this.xAxis.pane.center; return this.searchKDTree({ clientX: 180 + -180 / Math.PI * Math.atan2(a.chartX - c[0] - b.plotLeft, a.chartY - c[1] - b.plotTop) }) }; f.getConnectors = function (a, b, f, g) {
            var c, e, k, l, h, m, p, r; e = g ? 1 : 0; c = 0 <=
                b && b <= a.length - 1 ? b : 0 > b ? a.length - 1 + b : 0; b = 0 > c - 1 ? a.length - (1 + e) : c - 1; e = c + 1 > a.length - 1 ? e : c + 1; k = a[b]; e = a[e]; l = k.plotX; k = k.plotY; h = e.plotX; m = e.plotY; e = a[c].plotX; c = a[c].plotY; l = (1.5 * e + l) / 2.5; k = (1.5 * c + k) / 2.5; h = (1.5 * e + h) / 2.5; p = (1.5 * c + m) / 2.5; m = Math.sqrt(Math.pow(l - e, 2) + Math.pow(k - c, 2)); r = Math.sqrt(Math.pow(h - e, 2) + Math.pow(p - c, 2)); l = Math.atan2(k - c, l - e); p = Math.PI / 2 + (l + Math.atan2(p - c, h - e)) / 2; Math.abs(l - p) > Math.PI / 2 && (p -= Math.PI); l = e + Math.cos(p) * m; k = c + Math.sin(p) * m; h = e + Math.cos(Math.PI + p) * r; p = c + Math.sin(Math.PI +
                    p) * r; e = { rightContX: h, rightContY: p, leftContX: l, leftContY: k, plotX: e, plotY: c }; f && (e.prevPointCont = this.getConnectors(a, b, !1, g)); return e
        }; m(f, "buildKDTree", function (a) { this.chart.polar && (this.kdByAngle ? this.searchPoint = this.searchPointByAngle : this.options.findNearestPointBy = "xy"); a.apply(this) }); f.toXY = function (a) {
            var b, c = this.chart, g = a.plotX; b = a.plotY; a.rectPlotX = g; a.rectPlotY = b; b = this.xAxis.postTranslate(a.plotX, this.yAxis.len - b); a.plotX = a.polarPlotX = b.x - c.plotLeft; a.plotY = a.polarPlotY = b.y - c.plotTop;
            this.kdByAngle ? (c = (g / Math.PI * 180 + this.xAxis.pane.options.startAngle) % 360, 0 > c && (c += 360), a.clientX = c) : a.clientX = a.plotX
        }; p.spline && (m(p.spline.prototype, "getPointSpline", function (a, b, f, g) { this.chart.polar ? g ? (a = this.getConnectors(b, g, !0, this.connectEnds), a = ["C", a.prevPointCont.rightContX, a.prevPointCont.rightContY, a.leftContX, a.leftContY, a.plotX, a.plotY]) : a = ["M", f.plotX, f.plotY] : a = a.call(this, b, f, g); return a }), p.areasplinerange && (p.areasplinerange.prototype.getPointSpline = p.spline.prototype.getPointSpline));
        m(f, "translate", function (a) { var b = this.chart; a.call(this); if (b.polar && (this.kdByAngle = b.tooltip && b.tooltip.shared, !this.preventPostTranslate)) for (a = this.points, b = a.length; b--;)this.toXY(a[b]) }); m(f, "getGraphPath", function (a, b) {
            var c = this, g, d, e; if (this.chart.polar) { b = b || this.points; for (g = 0; g < b.length; g++)if (!b[g].isNull) { d = g; break } !1 !== this.options.connectEnds && void 0 !== d && (this.connectEnds = !0, b.splice(b.length, 0, b[d]), e = !0); u(b, function (a) { void 0 === a.polarPlotY && c.toXY(a) }) } g = a.apply(this, [].slice.call(arguments,
                1)); e && b.pop(); return g
        }); m(f, "animate", r); p.column && (p = p.column.prototype, p.polarArc = function (a, b, f, g) { var c = this.xAxis.center, e = this.yAxis.len; return this.chart.renderer.symbols.arc(c[0], c[1], e - b, null, { start: f, end: g, innerR: e - w(a, e) }) }, m(p, "animate", r), m(p, "translate", function (a) {
            var b = this.xAxis, c = b.startAngleRad, g, d, e; this.preventPostTranslate = !0; a.call(this); if (b.isRadial) for (g = this.points, e = g.length; e--;)d = g[e], a = d.barX + c, d.shapeType = "path", d.shapeArgs = {
                d: this.polarArc(d.yBottom, d.plotY, a, a +
                    d.pointWidth)
            }, this.toXY(d), d.tooltipPos = [d.plotX, d.plotY], d.ttBelow = d.plotY > b.center[1]
        }), m(p, "alignDataLabel", function (a, b, h, g, d, e) { this.chart.polar ? (a = b.rectPlotX / Math.PI * 180, null === g.align && (g.align = 20 < a && 160 > a ? "left" : 200 < a && 340 > a ? "right" : "center"), null === g.verticalAlign && (g.verticalAlign = 45 > a || 315 < a ? "bottom" : 135 < a && 225 > a ? "top" : "middle"), f.alignDataLabel.call(this, b, h, g, d, e)) : a.call(this, b, h, g, d, e) })); m(h, "getCoordinates", function (a, b) {
            var c = this.chart, g = { xAxis: [], yAxis: [] }; c.polar ? u(c.axes, function (a) {
                var d =
                    a.isXAxis, f = a.center, h = b.chartX - f[0] - c.plotLeft, f = b.chartY - f[1] - c.plotTop; g[d ? "xAxis" : "yAxis"].push({ axis: a, value: a.translate(d ? Math.PI - Math.atan2(h, f) : Math.sqrt(Math.pow(h, 2) + Math.pow(f, 2)), !0) })
            }) : g = a.call(this, b); return g
        }); m(a.Chart.prototype, "getAxes", function (c) { this.pane || (this.pane = []); u(a.splat(this.options.pane), function (b) { new a.Pane(b, this) }, this); c.call(this) }); m(a.Chart.prototype, "drawChartBox", function (a) { a.call(this); u(this.pane, function (a) { a.render() }) }); m(a.Chart.prototype, "get",
            function (c, b) { return a.find(this.pane, function (a) { return a.options.id === b }) || c.call(this, b) })
    })(x)
});
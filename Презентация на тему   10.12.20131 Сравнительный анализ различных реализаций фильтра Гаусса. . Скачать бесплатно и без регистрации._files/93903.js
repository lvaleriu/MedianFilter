var _up = [
                ['w', '240'],
                ['h', '400'],
                ['tagType', between_type],
                ['s', '93903'],
            ];
            var pix_id = 'tpix_93903';
            var _pix = document.getElementById ? document.getElementById(pix_id) : document.all[pix_id];
            
            if ( ! _pix ) {
                if (window.between_subid !== undefined) {
                    _up.push(['subid', between_subid])
                }
                if (window['btw_click3rd_93903'] !== undefined) {
                    _up.push(['click3rd', window['btw_click3rd_93903']])
                }
                if (document.cookie.indexOf('btw_force_show') >= 0) {
                    document.write(
                        "<script type='text/javascript'" +
                        "src='http://cache.betweendigital.com/code/force_show.js'></script>");
                } else {
                   document.write("<script type='text/javascript'" +
                      "src='http://ads.betweendigital.com/showad_full.js'></script>");
                }
            

var m_up = [['s', '94552'],['s_type', 'fullscreen']];
document.write("<script type='text/javascript' src='http://cache.betweendigital.com/code/mobile.v10.js'></script>");
}